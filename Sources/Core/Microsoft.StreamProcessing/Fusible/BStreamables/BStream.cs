namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TState"></typeparam>
    public abstract class BStream<TState, TPayload> : BStreamable<TState, TPayload>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        protected BStream(long period, long offset)
        {
            Period = period;
            Offset = offset;
        }

        /// <summary>
        /// 
        /// </summary>
        public long Period { get; }

        /// <summary>
        /// 
        /// </summary>
        public long Offset { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public abstract TPayload GetPayload(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public abstract long GetSyncTime(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public abstract long GetOtherTime(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public abstract bool GetBV(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public abstract int GetHash(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public abstract TState Next(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public long BeatCorrection(long t)
        {
            return t + Period - ((t - Offset) % Period);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public abstract bool IsDone(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract TState Init();
    }
}