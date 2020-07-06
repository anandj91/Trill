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
        private FSubWindow<TResult> _payload;

        /// <summary>
        /// 
        /// </summary>
        public SelectFWindow(FWindowable<TPayload> input, Expression<Func<TPayload, TResult>> selector)
            : base(input, input.Size, input.Period, input.Offset)
        {
            _selector = selector.Compile();
            _payload = new FSubWindow<TResult>(Length);
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
            get { return Input.Sync; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override FSubWindowable<long> Other
        {
            get { return Input.Other; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override FSubWindowable<bool> BV
        {
            get { return Input.BV; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetInputSize() => Input.GetInputSize();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<long, long>> GetInputSync() => Input.GetInputSync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<long, long>> GetOutputSync() => Input.GetOutputSync();

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
                    _payload.Data[i] = _selector(Input.Payload[i]);
                }
            }

            return len;
        }
    }
}