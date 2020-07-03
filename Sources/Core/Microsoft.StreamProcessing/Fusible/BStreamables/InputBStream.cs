namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TPayload"></typeparam>
    public class InputBStream<TKey, TPayload> : BStream<int, TPayload>
    {
        /// <summary>
        /// 
        /// </summary>
        public StreamMessage<TKey, TPayload> Batch { get; set; }
        private int Start;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        /// <param name="start"></param>
        public InputBStream(long period, long offset, int start = 0)
            : base(period, offset)
        {
            Start = start;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override TPayload GetPayload(int state) => Batch.payload.col[state];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override long GetSyncTime(int state) => Batch.vsync.col[state];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override long GetOtherTime(int state) => Batch.vother.col[state];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override bool GetBV(int state) => ((Batch.bitvector.col[state >> 6] & (1L << (state & 0x3f))) == 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override int GetHash(int state) => Batch.hash.col[state];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override int Next(int state)
        {
            while (!IsDone(state) && !GetBV(state))
            {
                ++state;
            }

            return state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override bool IsDone(int state) => state < Batch.Count;

        /// <summary>
        /// 
        /// </summary>
        public override int Init()
        {
            return Start;
        }
    }
}