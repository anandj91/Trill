namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TState"></typeparam>
    public interface BStreamable<TState, TPayload>
    {
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
        /// <returns></returns>
        public TPayload GetPayload(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetSyncTime(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetOtherTime(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetBV(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetHash(TState state);

        /// <summary>
        /// 
        /// </summary>
        public TState Next(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public long BeatCorrection(long t);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsDone(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TState Init();
    }
}