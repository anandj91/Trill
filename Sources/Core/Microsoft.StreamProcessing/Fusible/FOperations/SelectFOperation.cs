using System;
using System.Linq.Expressions;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class SelectFOperation<TPayload, TResult> : UnaryFOperation<TPayload, TResult>
    {
        private Expression<Func<TPayload, TResult>> _selector;

        /// <summary>
        /// 
        /// </summary>
        public SelectFOperation(FOperation<TPayload> input, Expression<Func<TPayload, TResult>> selector) : base(input)
        {
            _selector = selector;
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Size
        {
            get { return Input.Size; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Period
        {
            get { return Input.Period; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Offset
        {
            get { return Input.Offset; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override FWindowable<TResult> Compile(long offset, long size)
        {
            return new SelectFWindow<TPayload,TResult>(Input.Compile(offset, size), _selector);
        }
    }
}