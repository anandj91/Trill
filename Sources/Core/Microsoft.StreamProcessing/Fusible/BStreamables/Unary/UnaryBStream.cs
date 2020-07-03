namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <typeparam name="TIState"></typeparam>
    /// <typeparam name="TOState"></typeparam>
    public abstract class UnaryBStream<TIState, TInput, TOState, TOutput>
        : BStream<TOState, TOutput>
    {
        /// <summary>
        /// 
        /// </summary>
        protected BStreamable<TIState, TInput> Stream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        public UnaryBStream(BStreamable<TIState, TInput> stream, long period, long offset) : base(period, offset)
        {
            Stream = stream;
        }
    }
}