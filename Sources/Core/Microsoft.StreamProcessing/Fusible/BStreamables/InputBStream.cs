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
        private int Start;
        private int Idx;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        /// <param name="start"></param>
        public InputBStream(StreamMessage<TKey, TPayload> batch, long period, long offset, int start = 0)
            : base(period, offset)
        {
            Batch = batch;
            Start = start;
            Idx = -1;
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
        public override void Next()
        {
            while (!IsDone() && !GetBV())
            {
                ++Idx;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override BStreamable<TPayload> Clone()
        {
            return new InputBStream<TKey, TPayload>(Batch, Period, Offset, Idx);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsDone() => Idx < Batch.Count;

        /// <summary>
        /// 
        /// </summary>
        public override void Init()
        {
            Idx = Start;
        }
    }
}