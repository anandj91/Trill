namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TPayload"></typeparam>
    public class InputBStream<TKey, TPayload> : BStream<long, TPayload>
    {
        /// <summary>
        /// 
        /// </summary>
        public StreamMessage<TKey, TPayload> Batch {
            get
            {
                return Batch;
            }
            set
            {
                CurPos = NextPos;
                Batch = value;
                NextPos += Batch.Count;
            }
        }
        private long Start;
        private long NextPos;
        private long CurPos;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        /// <param name="start"></param>
        public InputBStream(long period, long offset, long start = 0)
            : base(period, offset)
        {
            Start = start;
            CurPos = 0;
            NextPos = 0;
        }

        private int Idx(long l) => (int) (l - CurPos);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TPayload GetPayload(long l) => Batch.payload.col[Idx(l)];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetSyncTime(long l) => Batch.vsync.col[Idx(l)];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetOtherTime(long l) => Batch.vother.col[Idx(l)];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool GetBV(long l) => ((Batch.bitvector.col[Idx(l) >> 6] & (1L << (Idx(l) & 0x3f))) == 0);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHash(long l) => Batch.hash.col[Idx(l)];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long Next(long l)
        {
            while (!IsDone(l) && !GetBV(l))
            {
                ++l;
            }

            return l;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsDone(long l) => Idx(l) < Batch.Count;

        /// <summary>
        /// 
        /// </summary>
        public override long Init()
        {
            return Start;
        }
    }
}