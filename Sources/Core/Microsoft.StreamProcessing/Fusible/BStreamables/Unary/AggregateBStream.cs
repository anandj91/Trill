using System;
using System.Collections.Generic;
using Microsoft.StreamProcessing.Aggregates;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TAggState"></typeparam>
    public class AggregateState<TAggState>
    {
        internal (long t, TAggState s) TopState;
        internal Queue<(long t, TAggState s)> States;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Window"></param>
        /// <param name="Period"></param>
        /// <param name="state"></param>
        public AggregateState(long Window, long Period, TAggState state)
        {
            States = new Queue<(long t, TAggState s)>((int) (Window / Period) + 1);
            TopState = (-1, state);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TAggState"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TIState"></typeparam>
    public class AggregateBStream<TIState, TPayload, TAggState, TResult>
        : ManyToOneBStream<TIState, TPayload, AggregateState<TAggState>, TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        protected long Window;

        private long Counter;
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
        /// <param name="window"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        public AggregateBStream(
            BStreamable<TIState, TPayload> stream,
            IAggregate<TPayload, TAggState, TResult> aggregate,
            long window, long period, long offset
        ) : base(stream, period, offset)
        {
            Window = window;
            Aggregate = aggregate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TResult GetPayload((TIState i, long t, AggregateState<TAggState> o) state)
            => Res(Diff(state.o.TopState.s, state.o.States.Peek().s));

        /// <summary>
        /// 
        /// </summary>
        public override (TIState i, long t, AggregateState<TAggState> o) Next(
            (TIState i, long t, AggregateState<TAggState> o) state
        )
        {
            long t;
            do
            {
                if (IsDone(state)) return state;
                t = Stream.GetSyncTime(state.i);
                while (state.o.States.Count > 0 && state.o.States.Peek().t < t - Window)
                {
                    state.o.States.Dequeue();
                }

                var newState = (t, Acc(state.o.TopState.s, t, Stream.GetPayload(state.i)));
                state.o.States.Enqueue(newState);
                state.o.TopState = newState;
            } while (t < Counter);

            Counter += Period;
            return state;
        }

        /// <summary>
        /// 
        /// </summary>
        public override (TIState i, long t, AggregateState<TAggState> o) Init()
        {
            var i = Stream.Init();
            Initialize = Aggregate.InitialState().Compile();
            Acc = Aggregate.Accumulate().Compile();
            Res = Aggregate.ComputeResult().Compile();
            Deacc = Aggregate.Deaccumulate().Compile();
            Diff = Aggregate.Difference().Compile();

            Counter = BeatCorrection(Stream.GetSyncTime(i));
            return (i, Counter, new AggregateState<TAggState>(Window, Period, Initialize()));
        }
    }
}