using System;
using System.Linq.Expressions;

namespace Microsoft.StreamProcessing.FOperationAPI
{
    /// <summary>
    /// 
    /// </summary>
    public static class FOperationExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <typeparam name="TPayload"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static FOperation<TResult> Select<TPayload, TResult>(
            this FOperation<TPayload> source,
            Expression<Func<TPayload, TResult>> selector
        )
        {
            Invariant.IsNotNull(source, nameof(source));
            Invariant.IsNotNull(selector, nameof(selector));

            return new SelectFOperation<TPayload, TResult>(source, selector);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="TPayload"></typeparam>
        /// <returns></returns>
        public static FOperation<TPayload> Where<TPayload>(
            this FOperation<TPayload> source,
            Expression<Func<TPayload, bool>> predicate
        )
        {
            Invariant.IsNotNull(source, nameof(source));
            Invariant.IsNotNull(predicate, nameof(predicate));

            return new WhereFOperation<TPayload>(source, predicate);
        }
    }
}