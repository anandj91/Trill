namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public abstract class BStream<TPayload> : BStreamable<TPayload>
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
        /// <returns></returns>
        public abstract TPayload GetPayload();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract long GetSyncTime();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract long GetOtherTime();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract bool GetBV();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract int GetHash();

        /// <summary>
        /// 
        /// </summary>
        public abstract void Next();

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
        /// <returns></returns>
        public abstract BStreamable<TPayload> Clone();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract bool IsDone();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract void Init();
    }
}