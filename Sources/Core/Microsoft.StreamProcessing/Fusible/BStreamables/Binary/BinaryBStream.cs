namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public class BinaryBState : BState
    {
        internal BState left;
        internal BState right;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public BinaryBState(BState left, BState right)
        {
            this.left = left;
            this.right = right;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TState"></typeparam>
    public abstract class BinaryBStream<TLeft, TRight, TState, TResult>
        : BStream<TState, TResult> where TState : BinaryBState
    {
        /// <summary>
        /// 
        /// </summary>
        protected BStreamable<TLeft> Left;

        /// <summary>
        /// 
        /// </summary>
        protected BStreamable<TRight> Right;

        /// <summary>
        /// 
        /// </summary>
        public BinaryBStream(BStreamable<TLeft> left, BStreamable<TRight> right, long period, long offset)
            : base(period, offset)
        {
            Left = left;
            Right = right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override bool _GetBV(TState state) => true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override int _GetHash(TState state) => 0;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool _Next(TState state)
        {
            if (!state.left.Ready || !state.right.Ready)
            {
                if (!state.left.Ready) state.left = Left.Next(state.left);
                if (!state.right.Ready) state.right = Right.Next(state.right);
            }
            else
            {
                if (Left.GetSyncTime(state.left) < Right.GetSyncTime(state.right))
                {
                    state.left = Left.Next(state.left);
                }
                else if (Left.GetSyncTime(state.left) >= Right.GetSyncTime(state.right))
                {
                    state.right = Right.Next(state.right);
                }
            }

            return ProcessNextItem(state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected abstract bool ProcessNextItem(TState state);
    }
}