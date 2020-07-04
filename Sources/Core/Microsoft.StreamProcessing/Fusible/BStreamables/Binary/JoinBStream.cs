using System;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class JoinBStream<TLeft, TRight, TResult> : BinaryBStream<TLeft, TRight, BinaryBState, TResult>
    {
        private Func<TLeft, TRight, TResult> Joiner;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="joiner"></param>
        public JoinBStream(BStreamable<TLeft> left, BStreamable<TRight> right, Func<TLeft, TRight, TResult> joiner)
            : base(left, right, left.Period, left.Offset)
        {
            Joiner = joiner;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override TResult _GetPayload(BinaryBState state)
            => Joiner(Left.GetPayload(state.left), Right.GetPayload(state.right));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override long _GetSyncTime(BinaryBState state)
            => Math.Max(Left.GetSyncTime(state.left), Right.GetSyncTime(state.right));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override long _GetOtherTime(BinaryBState state)
            => Math.Min(Left.GetOtherTime(state.left), Right.GetOtherTime(state.right));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override bool ProcessNextItem(BinaryBState state)
        {
            return Overlap(state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override bool _IsDone(BinaryBState state) => Left.IsDone(state.left) || Right.IsDone(state.right);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override BinaryBState _Init() => new BinaryBState(Left.Init(), Right.Init());

        private bool Overlap(BinaryBState state)
        {
            return state.left.Ready && state.right.Ready &&
                   (Math.Max(Left.GetSyncTime(state.left), Right.GetSyncTime(state.right)) <
                    Math.Min(Left.GetOtherTime(state.left), Right.GetOtherTime(state.right)));
        }
    }
}