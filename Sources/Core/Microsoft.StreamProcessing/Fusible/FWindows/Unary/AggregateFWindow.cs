using System;
using Microsoft.StreamProcessing.Aggregates;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TAggState"></typeparam>
    public class AggregateFWindow<TPayload, TAggState, TResult> : UnaryFWindow<TPayload, TResult>
    {
        private long _window;
        private IAggregate<TPayload, TAggState, TResult> _aggregate;
        private Func<TAggState> _init;
        private Func<TAggState, long, TPayload, TAggState> _acc;
        private Func<TAggState, TResult> _res;
        private Func<TAggState, long, TPayload, TAggState> _deacc;
        private Func<TAggState, TAggState, TAggState> _diff;
        private TAggState _state;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="aggregate"></param>
        /// <param name="window"></param>
        public AggregateFWindow(
            FWindowable<TPayload> input,
            IAggregate<TPayload, TAggState, TResult> aggregate,
            long window
        ) : base(input, input.Size, window, input.Offset)
        {
            Invariant.IsTrue(input.Size % window == 0, "Input size need to be a multiple of window");
            _window = window;
            _aggregate = aggregate;
            _init = _aggregate.InitialState().Compile();
            _acc = _aggregate.Accumulate().Compile();
            _res = _aggregate.ComputeResult().Compile();
            _deacc = _aggregate.Deaccumulate().Compile();
            _diff = _aggregate.Difference().Compile();
            _state = _init();
            // TODO: Need to handle gaps. Currently BV is always true.
            _BV = new BVFSubWindow(Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override int _Compute()
        {
            var ilen = Input.Compute();
            int olen = 0;

            var period = Period;
            var ipayload = Input.Payload.Data;
            var ipayloadOffset = Input.Payload.Offset;
            var ibvOffset = Input.BV.Offset;
            var iotherOffset = Input.Other.Offset;
            var payload = Payload.Data;
            var payloadOffset = Payload.Offset;
            var syncOffset = Sync.Offset;
            var otherOffset = Other.Offset;

            unsafe
            {
                fixed (long* ibv = Input.BV.Data)
                fixed (long* ivother = Input.Other.Data)
                {
                    for (int i = 0; i < ilen; i++)
                    {
                        // TODO: Flush at the end of stream
                        var ibi = ibvOffset + i;
                        if ((ibv[ibi >> 6] & (1L << (ibi & 0x3f))) == 0)
                        {
                            var ipi = ipayloadOffset + i;
                            var isi = iotherOffset + i;

                            var item = ipayload[ipi];
                            var other = ivother[isi];
                            _state = _acc(_state, other, item);
                            if (other % period == 0)
                            {
                                var result = _res(_state);
                                _state = _init();
                                payload[payloadOffset + olen] = result;
                                _Sync.Data[syncOffset + olen] = other - period;
                                _Other.Data[otherOffset + olen] = other;
                                olen++;
                            }
                        }
                    }
                }
            }

            return olen;
        }
    }
}