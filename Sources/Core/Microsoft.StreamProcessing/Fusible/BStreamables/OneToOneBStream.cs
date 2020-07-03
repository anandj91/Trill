namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public abstract class OneToOneBStream<TState, TInput, TOutput> : UnaryBStream<TState, TInput, TState, TOutput>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        public OneToOneBStream(BStreamable<TState, TInput> stream, long period, long offset)
            : base(stream, period, offset)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override long GetSyncTime(TState state) => Stream.GetSyncTime(state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override long GetOtherTime(TState state) => Stream.GetOtherTime(state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override bool IsDone(TState state) => Stream.IsDone(state);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TState Init() => Stream.Init();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override int GetHash(TState state) => Stream.GetHash(state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override bool GetBV(TState state) => true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override TState Next(TState state) => Stream.Next(state);
    }
}