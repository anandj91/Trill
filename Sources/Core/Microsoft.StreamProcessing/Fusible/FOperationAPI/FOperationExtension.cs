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
            var p = new StreamProperties<Empty, TPayload>(
                false, true, source.Period, true, period, offset, false, true, true, true,
                EqualityComparerExpression<Empty>.Default, EqualityComparerExpression<TPayload>.Default,
                ComparerExpression<Empty>.Default, ComparerExpression<TPayload>.Default, null, null, null
            );
            return new AggregateFOperation<TPayload, TAggState, TResult>(
                source, aggregate(new Window<Empty, TPayload>(p)), window, period, offset
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <typeparam name="TPayload"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static FOperation<TResult> Multicast<TPayload, TResult>(
            this FOperation<TPayload> source,
            Func<FOperation<TPayload>, FOperation<TResult>> selector
        )
        {
            Invariant.IsNotNull(source, nameof(source));
            Invariant.IsNotNull(selector, nameof(selector));

            return selector(new MulticastFOperation<TPayload>(source));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="joiner"></param>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static FOperation<TResult> Join<TLeft, TRight, TResult>(
            this FOperation<TLeft> left,
            FOperation<TRight> right,
            Expression<Func<TLeft, TRight, TResult>> joiner
        )
        {
            Invariant.IsNotNull(left, nameof(left));
            Invariant.IsNotNull(right, nameof(right));

            return new JoinFOperation<TLeft, TRight, TResult>(left, right, joiner);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="joiner"></param>
        /// <typeparam name="TPayload"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static FOperation<TResult> ConsecutivePairs<TPayload, TResult>(
            this FOperation<TPayload> input,
            Expression<Func<TPayload, TPayload, TResult>> joiner
        )
        {
            return new PairFOperation<TPayload, TResult>(input, joiner);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fop"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static IStreamable<Empty, TResult> FuseEnd<TResult>(this FOperation<TResult> fop)
        {
            return new FEndStreamable<TResult>(fop);
        }
    }
}