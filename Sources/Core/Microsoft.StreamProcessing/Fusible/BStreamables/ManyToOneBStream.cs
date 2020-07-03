namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TIState"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOState"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public abstract class ManyToOneBStream<TIState, TInput, TOState, TOutput>
        : UnaryBStream<TIState, TInput, (TIState i, long t, TOState o), TOutput>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        public ManyToOneBStream(BStreamable<TIState, TInput> stream, long period, long offset)
            : base(stream, period, offset)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool GetBV((TIState i, long t, TOState o) state) => true;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHash((TIState i, long t, TOState o) state) => 0;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetSyncTime((TIState i, long t, TOState o) state) => state.t;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetOtherTime((TIState i, long t, TOState o) state) => state.t + Period;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override bool IsDone((TIState i, long t, TOState o) state) => Stream.IsDone(state.i);
    }
}