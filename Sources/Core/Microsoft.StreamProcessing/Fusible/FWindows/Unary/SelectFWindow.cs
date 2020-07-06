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
        private Func<TPayload, TResult> _selector;

        /// <summary>
        /// 
        /// </summary>
        public SelectFWindow(FWindowable<TPayload> input, Expression<Func<TPayload, TResult>> selector)
            : base(input, input.Size, input.Period, input.Offset)
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
        protected override int _Compute()
        {
            var len = Input.Compute();

            for (int i = 0; i < len; i++)
            {
                if (BV[i])
                {
                    _Payload.Data[i] = _selector(Input.Payload[i]);
                }
            }

            return len;
        }
    }
}