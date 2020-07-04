namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public class InputBState : BState
    {
        /// <summary>
        /// 
        /// </summary>
        public long l;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        public InputBState(long l)
        {
            this.l = l;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TPayload"></typeparam>
    public class InputBStream<TKey, TPayload> : BStream<InputBState, TPayload>
    {
        /// <summary>
        /// 
        /// </summary>
        public StreamMessage<TKey, TPayload> Batch;

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

        private int Idx(InputBState state) => (int) (state.l - CurPos);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override TPayload _GetPayload(InputBState state) => Batch.payload.col[Idx(state)];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override long _GetSyncTime(InputBState state) => Batch.vsync.col[Idx(state)];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override long _GetOtherTime(InputBState state) => Batch.vother.col[Idx(state)];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool _GetBV(InputBState state)
            => ((Batch.bitvector.col[Idx(state) >> 6] & (1L << (Idx(state) & 0x3f))) == 0);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override int _GetHash(InputBState state) => Batch.hash.col[Idx(state)];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool _Next(InputBState state)
        {
            state.l++;
            return !_IsDone(state) && _GetBV(state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool _IsDone(InputBState state)
            => (Idx(state) >= Batch.Count || GetSyncTime(state) > StreamEvent.MaxSyncTime);

        /// <summary>
        /// 
        /// </summary>
        protected override InputBState _Init()
        {
            var state = new InputBState(Start);
            state.Ready = true;
            return state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetBatch(StreamMessage<TKey, TPayload> value)
        {
            CurPos = NextPos;
            Batch = value;
            NextPos += Batch.Count;
        }
    }
}