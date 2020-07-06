using System;
using System.Linq.Expressions;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class WhereFWindow<TPayload> : UnaryFWindow<TPayload, TPayload>
    {
        private Func<TPayload, bool> _filter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="filter"></param>
        public WhereFWindow(FWindowable<TPayload> input, Expression<Func<TPayload, bool>> filter)
            : base(input, input.Size, input.Period, input.Offset)
        {
            _filter = filter.Compile();
            _Payload = Input.Payload as FSubWindow<TPayload>;
            _Sync = Input.Sync as FSubWindow<long>;
            _Other = Input.Other as FSubWindow<long>;
            _BV = new BVFSubWindow(Length);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int _Compute()
        {
            var len = Input.Compute();

            /* Init bits to zero */
            for (int i = 0; i < BV.Data.Length; i++)
            {
                BV.Data[i] = 0;
            }

            /* Set bits */
            for (int i = 0; i < len; i++)
            {
                if (!Input.BV[i] || !_filter(Payload[i]))
                    BV.Data[i >> 6] |= (1L << (i & 0x3f));
            }

            /* Set residual bits */
            for (int i = len; i < BV.Data.Length; i++)
            {
                BV.Data[i >> 6] |= (1L << (i & 0x3f));
            }

            return len;
        }
    }
}