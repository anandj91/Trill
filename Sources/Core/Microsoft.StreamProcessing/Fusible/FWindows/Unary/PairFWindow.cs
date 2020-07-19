using System;
using System.Linq.Expressions;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class PairFWindow<TPayload, TResult> : UnaryFWindow<TPayload, TResult>
    {
        private Func<TPayload, TPayload, TResult> _joiner;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="joiner"></param>
        public PairFWindow(FWindowable<TPayload> input, Expression<Func<TPayload, TPayload, TResult>> joiner)
            : base(input, input.Size, input.Period, input.Offset, -1)
        {
            Invariant.IsPositive(input.Duration, "Input duration");
            _joiner = joiner.Compile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool _Init()
        {
            var ret = Input.Init();
            if (ret)
            {
                ret &= InitInput();
            }

            return ret;
        }

        private bool InitInput()
        {
            Input.Compute();
            return Input.Slide(Input.SyncTime);
        }

        private int PreCompute()
        {
            Input.Sync.Copy(Sync);
            Input.Other.Copy(Other);
            Input.BV.Copy(BV);

            var period = Period;
            var ipayload = Input.Payload.Data;
            var ipayloadOffset = Input.Payload.Offset;
            var payload = Payload.Data;
            var payloadOffset = Payload.Offset;
            var ibvOffset = Input.BV.Offset;
            var other = Other.Data;
            var otherOffset = Other.Offset;
            var isyncOffset = Input.Sync.Offset;
            var syncTime = Input.Sync.Data[isyncOffset];

            int previ = -1;
            unsafe
            {
                fixed (long* bv = Input.BV.Data)
                {
                    for (int i = 0; i < Length; i++)
                    {
                        var ibi = ibvOffset + i;
                        if ((bv[ibi >> 6] & (1L << (ibi & 0x3f))) == 0)
                        {
                            if (previ != -1)
                            {
                                var prevpi = ipayloadOffset + previ;
                                var curpi = ipayloadOffset + i;
                                var pi = payloadOffset + previ;
                                var otheri = otherOffset + previ;
                                payload[pi] = _joiner(ipayload[prevpi], ipayload[curpi]);
                                other[otheri] = syncTime;
                            }

                            previ = i;
                        }

                        syncTime += period;
                    }
                }
            }

            SyncTime = syncTime;

            return previ;
        }

        private void PostCompute(TPayload prevPayload, int previ)
        {
            var period = Period;
            var ipayload = Input.Payload.Data;
            var ipayloadOffset = Input.Payload.Offset;
            var payload = Payload.Data;
            var payloadOffset = Payload.Offset;
            var ibvOffset = Input.BV.Offset;
            var other = Other.Data;
            var otherOffset = Other.Offset;
            var isyncOffset = Input.Sync.Offset;
            var syncTime = Input.Sync.Data[isyncOffset];

            unsafe
            {
                fixed (long* bv = Input.BV.Data)
                {
                    for (int i = 0; i < Length; i++)
                    {
                        var ibi = ibvOffset + i;
                        if ((bv[ibi >> 6] & (1L << (ibi & 0x3f))) == 0)
                        {
                            var curpi = ipayloadOffset + i;
                            var pi = payloadOffset + previ;
                            var otheri = otherOffset + previ;
                            payload[pi] = _joiner(prevPayload, ipayload[curpi]);
                            other[otheri] = syncTime;
                            break;
                        }

                        syncTime += period;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override int _Compute()
        {
            int previ = PreCompute();
            var ipayload = Input.Payload.Data;
            var ipayloadOffset = Input.Payload.Offset;
            var prevPayload = ipayload[ipayloadOffset + previ];
            Input.Compute();
            PostCompute(prevPayload, previ);
            return Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tsync"></param>
        /// <returns></returns>
        protected override bool _Slide(long tsync)
        {
            var ret = true;
            ret &= Input.Slide(tsync);

            if (ret && tsync > SyncTime)
            {
                ret &= InitInput();
            }

            return ret;
        }
    }
}