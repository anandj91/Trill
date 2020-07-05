using System;
using System.Linq.Expressions;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class SelectPayloadFSubWindow<TPayload, TResult> : FSubWindowable<TResult>
    {
        private FSubWindowable<TPayload> _payload;
        private Func<TPayload, TResult> _selector;

        /// <summary>
        /// 
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public TResult this[int i] => _selector(_payload[i]);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="payload"></param>
        /// <param name="selector"></param>
        public SelectPayloadFSubWindow(int size, FSubWindowable<TPayload> payload, Func<TPayload, TResult> selector)
        {
            Size = size;
            _payload = payload;
            _selector = selector;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class SelectFWindow<TPayload, TResult> : UnaryFWindow<TPayload, TResult>
    {
        private SelectPayloadFSubWindow<TPayload, TResult> _payload;
        /// <summary>
        /// 
        /// </summary>
        public SelectFWindow(FWindowable<TPayload> input, Expression<Func<TPayload, TResult>> selector)
            : base(input, input.Size, input.Period, input.Offset)
        {
            _payload = new SelectPayloadFSubWindow<TPayload, TResult>(
                Input.Payload.Size, Input.Payload, selector.Compile()
            );
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
        protected override void _Compute() => Input.Compute();
    }
}