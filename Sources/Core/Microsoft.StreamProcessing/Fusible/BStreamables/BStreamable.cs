namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public interface BStreamable<TPayload>
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
        public TPayload GetPayload();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetSyncTime();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetOtherTime();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetBV();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetHash();

        /// <summary>
        /// 
        /// </summary>
        public void Next();

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
        public BStreamable<TPayload> Clone();
    }
}