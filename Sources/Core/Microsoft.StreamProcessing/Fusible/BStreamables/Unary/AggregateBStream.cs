using System;
using System.Collections.Generic;
using Microsoft.StreamProcessing.Aggregates;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class AggregateBStream<TPayload, TState, TResult> : UnaryBStream<TPayload, TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        protected long Window;
        private long Counter;
        private Func<TState> Init;
        private Func<TState, long, TPayload, TState> Acc;
        private Func<TState, TResult> Res;
        private Func<TState, long, TPayload, TState> Deacc;
        private Func<TState, TState, TState> Diff;

        private (long t, TState s) State;
        private Queue<(long t, TState s)> States;
        private int Size;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="aggregate"></param>
        /// <param name="window"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        public AggregateBStream(
            BStreamable<TPayload> stream,
            IAggregate<TPayload, TState, TResult> aggregate,
            long window, long period, long offset
        ) : base(stream, period, offset)
        {
            Window = window;
            Counter = BeatCorrection(stream.GetSyncTime());
            Init = aggregate.InitialState().Compile();
            Acc = aggregate.Accumulate().Compile();
            Res = aggregate.ComputeResult().Compile();
            Deacc = aggregate.Deaccumulate().Compile();
            Diff = aggregate.Difference().Compile();

            Size = (int) (window / period) + 1;
            States = new Queue<(long t, TState s)>(Size);
            State = (-1, Init());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool GetBV() => true;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHash() => 0;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TResult GetPayload() => Res(Diff(State.s, States.Peek().s));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetSyncTime() => Counter;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetOtherTime() => Counter + Period;

        /// <summary>
        /// 
        /// </summary>
        public override void Next()
        {
            long t;
            do
            {
                Stream.Next();
                t = Stream.GetSyncTime();
                while (States.Count > 0 && States.Peek().t < t - Window)
                {
                    States.Dequeue();
                }

                State = (t, Acc(State.s, t, Stream.GetPayload()));
                States.Enqueue(State);
            } while (t < Counter);

            Counter += Period;
        }
    }
}