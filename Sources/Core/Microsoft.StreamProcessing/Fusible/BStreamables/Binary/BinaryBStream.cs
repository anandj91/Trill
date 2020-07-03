namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TLState"></typeparam>
    /// <typeparam name="TRState"></typeparam>
    /// <typeparam name="TState"></typeparam>
    public abstract class BinaryBStream<TLState, TLeft, TRState, TRight, TState, TResult>
        : BStream<(TLState l, TRState r, TState o), TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        protected BStreamable<TLState, TLeft> Left;

        /// <summary>
        /// 
        /// </summary>
        protected BStreamable<TRState, TRight> Right;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        public BinaryBStream(BStreamable<TLState, TLeft> left, BStreamable<TRState, TRight> right,
            long period, long offset
        )
            : base(period, offset)
        {
            Left = left;
            Right = right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool GetBV((TLState l, TRState r, TState o) state) => true;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHash((TLState l, TRState r, TState o) state) => 0;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsDone((TLState l, TRState r, TState o) state)
            => Left.IsDone(state.l) && Right.IsDone(state.r);
    }
}