using System;
using Microsoft.StreamProcessing.Aggregates;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TAggState"></typeparam>
    public class AggregateState<TAggState> : UnaryBState
    {
        internal long syncTime;
        internal TAggState curState;
        internal TAggState prevState;

        /// <summary>
        /// 
        /// </summary>
        public AggregateState(BState i, long time, TAggState state) : base(i)
        {
            this.curState = state;
            this.prevState = state;
            this.syncTime = time;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TAggState"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class AggregateBStream<TPayload, TAggState, TResult>
        : UnaryBStream<TPayload, AggregateState<TAggState>, TResult>
    {
        private IAggregate<TPayload, TAggState, TResult> Aggregate;
        private Func<TAggState> Initialize;
        private Func<TAggState, long, TPayload, TAggState> Acc;
        private Func<TAggState, TResult> Res;
        private Func<TAggState, long, TPayload, TAggState> Deacc;
        private Func<TAggState, TAggState, TAggState> Diff;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="aggregate"></param>
        public AggregateBStream(
            TumblingWindowBStream<TPayload> stream,
            IAggregate<TPayload, TAggState, TResult> aggregate
        ) : base(stream, stream.Period, stream.Offset)
        {
            Aggregate = aggregate;
            Initialize = Aggregate.InitialState().Compile();
            Acc = Aggregate.Accumulate().Compile();
            Res = Aggregate.ComputeResult().Compile();
            Deacc = Aggregate.Deaccumulate().Compile();
            Diff = Aggregate.Difference().Compile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override AggregateState<TAggState> _Init()
            => new AggregateState<TAggState>(Stream.Init(), -1, Initialize());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override TResult _GetPayload(AggregateState<TAggState> state) => Res(state.prevState);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override TResult Selector(TPayload payload)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override bool ProcessNextItem(AggregateState<TAggState> state)
        {
            bool res = false;
            var item = Stream.GetPayload(state.i);
            var sync = Stream.GetSyncTime(state.i);

            if (state.syncTime < sync)
            {
                state.syncTime = BeatCorrection(sync);
                state.prevState = state.curState;
                state.curState = Initialize();
                res = true;
            }

            state.curState = Acc(state.curState, sync, item);
            return res;
        }
    }
}