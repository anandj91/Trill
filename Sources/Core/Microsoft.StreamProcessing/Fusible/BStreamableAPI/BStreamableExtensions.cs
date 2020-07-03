using System;
using System.Linq.Expressions;
using Microsoft.StreamProcessing.Aggregates;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public static class BStreamableExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TPayload"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static BStreamable<TState, TResult> Select<TState, TPayload, TResult>(
            this BStreamable<TState, TPayload> source,
            Expression<Func<TPayload, TResult>> selector)
        {
            Invariant.IsNotNull(source, nameof(source));
            Invariant.IsNotNull(selector, nameof(selector));

            return new SelectBStream<TState, TPayload, TResult>(source, selector.Compile());
        }

        /// <summary>
        /// Performs multicast over a streamable. This allows query writers to execute multiple subqueries over the same physical input stream.
        /// </summary>
        public static BStreamable<TOState, TResult> Multicast<TIState, TPayload, TOState, TResult>(
            this BStreamable<TIState, TPayload> source,
            Func<BStreamable<TIState, TPayload>, BStreamable<TOState, TResult>> selector
        )
        {
            Invariant.IsNotNull(source, nameof(source));
            Invariant.IsNotNull(selector, nameof(selector));

            return selector(source);
        }

        /// <summary>
        /// Applies an aggregate to snapshot windows on the input stream.
        /// </summary>
        public static BStreamable<(TIState i, long t, AggregateState<TAggState> o), TResult> Aggregate
            <TIState, TPayload, TAggState, TResult>(
                this BStreamable<TIState, TPayload> source,
                Func<Window<Empty, TPayload>, IAggregate<TPayload, TAggState, TResult>> aggregate,
                long window, long period, long offset = 0)
        {
            Invariant.IsNotNull(source, nameof(source));
            Invariant.IsNotNull(aggregate, nameof(aggregate));
            var p = new StreamProperties<Empty, TPayload>(false, true, source.Period, true, source.Period,
                source.Offset,
                false, true, true, true,
                EqualityComparerExpression<Empty>.Default, EqualityComparerExpression<TPayload>.Default,
                ComparerExpression<Empty>.Default, ComparerExpression<TPayload>.Default, null, null, null);
            return new AggregateBStream<TIState, TPayload, TAggState, TResult>(
                source, aggregate(new Window<Empty, TPayload>(p)), window, period, offset
            );
        }

        /// <summary>
        /// Performs a cross-product between 2 streams
        /// </summary>
        public static BStreamable<(TLState l, TRState r, Empty o), TResult> Join
            <TLState, TLeft, TRState, TRight, TResult>(
                this BStreamable<TLState, TLeft> left,
                BStreamable<TRState, TRight> right,
                Func<TLeft, TRight, TResult> resultSelector)
        {
            Invariant.IsNotNull(left, nameof(left));
            Invariant.IsNotNull(right, nameof(right));

            return new JoinBStream<TLState, TLeft, TRState, TRight, TResult>(
                left, right, resultSelector, left.Period, left.Offset
            );
        }
    }
}