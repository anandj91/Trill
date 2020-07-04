namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TState"></typeparam>
    public abstract class BStream<TState, TPayload> : BStreamable<TPayload> where TState : BState
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
        public TPayload GetPayload(BState state) => _GetPayload((TState) state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public long GetSyncTime(BState state) => _GetSyncTime((TState) state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public long GetOtherTime(BState state) => _GetOtherTime((TState) state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool GetBV(BState state) => _GetBV((TState) state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public int GetHash(BState state) => _GetHash((TState) state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public BState Next(BState state)
        {
            state.Ready = false;
            while (!IsDone(state) && !state.Ready)
            {
                state.Ready = _Next((TState) state);
            }
            return state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool IsDone(BState state) => _IsDone((TState) state);
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BState Init() => _Init();

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
        protected abstract TPayload _GetPayload(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected abstract long _GetSyncTime(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected abstract long _GetOtherTime(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected abstract bool _GetBV(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected abstract int _GetHash(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected abstract bool _Next(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected abstract bool _IsDone(TState state);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract TState _Init();
    }
}