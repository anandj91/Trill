using System;
using System.Linq.Expressions;
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
        private FSubWindow<TResult> _payload;
        private FSubWindow<long> _sync;
        private FSubWindow<long> _other;
        private FSubWindow<bool> _bv;

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
            _payload = new FSubWindow<TResult>(Length);
            _sync = new FSubWindow<long>(Length);
            _other = new FSubWindow<long>(Length);
            // TODO: Deal with gaps
            _bv = new FSubWindow<bool>(Length);

            _aggregate = aggregate;
            _init = _aggregate.InitialState().Compile();
            _acc = _aggregate.Accumulate().Compile();
            _res = _aggregate.ComputeResult().Compile();
            _deacc = _aggregate.Deaccumulate().Compile();
            _diff = _aggregate.Difference().Compile();
            _state = _init();
        }

        /// <summary>
        /// 
        /// </summary>
        public override FSubWindowable<TResult> Payload
        {
            get { return _payload; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override FSubWindowable<long> Sync
        {
            get { return _sync; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override FSubWindowable<long> Other
        {
            get { return _other; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override FSubWindowable<bool> BV
        {
            get { return _bv; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override long GetInputSize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Expression<Func<long, long>> GetInputSync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Expression<Func<long, long>> GetOutputSync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override int _Compute()
        {
            var ilen = Input.Compute();
            int olen = 0;

            for (int i = 0; i < ilen; i++)
            {
                // TODO: Flush at the end of stream
                if (Input.BV[i])
                {
                    var item = Input.Payload[i];
                    var sync = Input.Sync[i];
                    if (sync % Period == 0)
                    {
                        var result = _res(_state);
                        _state = _init();
                        _payload.Data[olen] = result;
                        _sync.Data[olen] = sync;
                        _other.Data[olen] = sync + Period;
                        _bv.Data[olen] = true;
                        olen++;
                    }

                    _state = _acc(_state, sync, item);
                }
            }

            return olen;
        }
    }
}