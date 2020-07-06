using System;
using System.Linq.Expressions;
using Microsoft.StreamProcessing.Aggregates;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="aggregate"></param>
        /// <param name="window"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        /// <typeparam name="TPayload"></typeparam>
        /// <typeparam name="TAggState"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static FOperation<TResult> Aggregate<TPayload, TAggState, TResult>(
            this FOperation<TPayload> source,
            Func<Window<Empty, TPayload>, IAggregate<TPayload, TAggState, TResult>> aggregate,
            long window, long period, long offset = 0
        )
        {
            Invariant.IsNotNull(source, nameof(source));
            Invariant.IsNotNull(aggregate, nameof(aggregate));
            var tmp = source.Compile(1);
            var p = new StreamProperties<Empty, TPayload>(
                false, true, tmp.Period, true, period, offset, false, true, true, true,
                EqualityComparerExpression<Empty>.Default, EqualityComparerExpression<TPayload>.Default,
                ComparerExpression<Empty>.Default, ComparerExpression<TPayload>.Default, null, null, null
            );
            return new AggregateFOperation<TPayload, TAggState, TResult>(
                source, aggregate(new Window<Empty, TPayload>(p)), window, period, offset
            );
        }
    }
}