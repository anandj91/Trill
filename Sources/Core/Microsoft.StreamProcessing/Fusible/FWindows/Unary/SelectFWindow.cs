using System;
using System.Linq.Expressions;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class SelectFWindow<TPayload, TResult> : UnaryFWindow<TPayload, TResult>
    {
        private Func<long, TPayload, TResult> _selector;

        /// <summary>
        /// 
        /// </summary>
        public SelectFWindow(FWindowable<TPayload> input, Expression<Func<long, TPayload, TResult>> selector)
            : base(input, input.Size, input.Period, input.Offset, input.Duration)
        {
            _selector = selector.Compile();
            _Payload = new FSubWindow<TResult>(Length);
            _Sync = Input.Sync as FSubWindow<long>;
            _Other = Input.Other as FSubWindow<long>;
            _BV = Input.BV as BVFSubWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override bool _Init()
        {
            var ret = Input.Init();
            SyncTime = Input.SyncTime;
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int _Compute()
        {
            var len = Input.Compute();

            var period = Period;
            var ipayload = Input.Payload.Data;
            var ipayloadOffset = Input.Payload.Offset;
            var opayload = Payload.Data;
            var opayloadOffset = Payload.Offset;
            var ibvOffset = Input.BV.Offset;

            unsafe
            {
                fixed (long* bv = Input.BV.Data)
                {
                    for (int i = 0; i < Length; i++)
                    {
                        var ibi = ibvOffset + i;
                        if ((bv[ibi >> 6] & (1L << (ibi & 0x3f))) == 0)
                        {
                            var pi = ipayloadOffset + i;
                            var po = opayloadOffset + i;
                            opayload[po] = _selector(SyncTime, ipayload[pi]);
                        }

                        SyncTime += period;
                    }
                }
            }

            return len;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tsync"></param>
        /// <returns></returns>
        protected override bool _Slide(long tsync)
        {
            var ret = Input.Slide(tsync);
            SyncTime = Input.SyncTime;
            return ret;
        }
    }
}