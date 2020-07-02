namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TPayload"></typeparam>
    public class InputBStream<TKey, TPayload> : BStream<TPayload>
    {
        private StreamMessage<TKey, TPayload> Batch;
        private int Idx;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        /// <param name="idx"></param>
        public InputBStream(StreamMessage<TKey, TPayload> batch, long period, long offset, int idx = 0)
            : base(period, offset)
        {
            Batch = batch;
            Idx = idx - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TPayload GetPayload() => Batch.payload.col[Idx];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetSyncTime() => Batch.vsync.col[Idx];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetOtherTime() => Batch.vother.col[Idx];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool GetBV() => ((Batch.bitvector.col[Idx >> 6] & (1L << (Idx & 0x3f))) == 0);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHash() => Batch.hash.col[Idx];

        /// <summary>
        /// 
        /// </summary>
        public override void Next() => Idx++;
    }
}