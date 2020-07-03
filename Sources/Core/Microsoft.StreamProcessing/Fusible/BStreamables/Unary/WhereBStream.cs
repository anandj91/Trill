using System;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TState"></typeparam>
    public class WhereBStream<TState, TPayload> : OneToOneBStream<TState, TPayload, TPayload>
    {
        private Func<TPayload, bool> Filter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filter"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        public WhereBStream(BStreamable<TState, TPayload> stream, Func<TPayload, bool> filter, long period, long offset)
            : base(stream, period, offset)
        {
            Filter = filter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override TPayload GetPayload(TState state) => Stream.GetPayload(state);

        /// <summary>
        /// 
        /// </summary>
        public override TState Next(TState state)
        {
            do
            {
                if (IsDone(state)) return state;
                state = Stream.Next(state);
            } while (!Stream.GetBV(state) || Filter(Stream.GetPayload(state)));

            return state;
        }
    }
}