using System;

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
    public class JoinBStream<TLState, TLeft, TRState, TRight, TResult>
        : BinaryBStream<TLState, TLeft, TRState, TRight, Empty, TResult>
    {
        private Func<TLeft, TRight, TResult> Joiner;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="joiner"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        public JoinBStream(
            BStreamable<TLState, TLeft> left,
            BStreamable<TRState, TRight> right,
            Func<TLeft, TRight, TResult> joiner,
            long period, long offset)
            : base(left, right, period, offset)
        {
            Joiner = joiner;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TResult GetPayload((TLState l, TRState r, Empty o) state)
            => Joiner(Left.GetPayload(state.l), Right.GetPayload(state.r));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetSyncTime((TLState l, TRState r, Empty o) state)
            => Math.Max(Left.GetSyncTime(state.l), Right.GetSyncTime(state.r));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetOtherTime((TLState l, TRState r, Empty o) state)
            => Math.Min(Left.GetOtherTime(state.l), Right.GetOtherTime(state.r));

        /// <summary>
        /// 
        /// </summary>
        public override (TLState l, TRState r, Empty o) Next(
            (TLState l, TRState r, Empty o) state
        )
        {
            while (!IsDone(state) && !Overlap(state))
            {
                if (Left.GetSyncTime(state.l) < Right.GetSyncTime(state.r))
                {
                    state.l = Left.Next(state.l);
                }
                else
                {
                    state.r = Right.Next(state.r);
                }
            }

            return state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override (TLState l, TRState r, Empty o) Init() => (Left.Init(), Right.Init(), default);

        private bool Overlap((TLState l, TRState r, Empty o) state)
        {
            return (Math.Max(Left.GetSyncTime(state.l), Right.GetSyncTime(state.r)) <
                    Math.Min(Left.GetOtherTime(state.l), Right.GetOtherTime(state.r)));
        }
    }
}