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
        /// <returns></returns>
        public override FWindowable<TResult> Compile(int factor)
        {
            return new SelectFWindow<TPayload, TResult>(Input.Compile(factor), _selector);
        }
    }
}