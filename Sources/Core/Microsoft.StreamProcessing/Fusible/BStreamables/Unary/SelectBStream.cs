using System;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <typeparam name="TState"></typeparam>
    public class SelectBStream<TState, TInput, TOutput> : OneToOneBStream<TState, TInput, TOutput>
    {
        private Func<TInput, TOutput> Selector;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="selector"></param>
        public SelectBStream(BStreamable<TState, TInput> stream, Func<TInput, TOutput> selector)
            : base(stream, stream.Period, stream.Offset)
        {
            Selector = selector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override TOutput GetPayload(TState state) => Selector(Stream.GetPayload(state));
    }
}