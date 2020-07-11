using System;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public abstract class BinaryFWindow<TLeft, TRight, TResult> : FWindow<TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        protected FWindowable<TLeft> Left;

        /// <summary>
        /// 
        /// </summary>
        protected FWindowable<TRight> Right;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="right"></param>
        /// <param name="size"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        /// <param name="left"></param>
        public BinaryFWindow(FWindowable<TLeft> left, FWindowable<TRight> right, long size, long period, long offset)
            : base(size, period, offset)
        {
            Left = left;
            Right = right;
        }

        /// <summary>
        /// 
        /// </summary>
        public override long SyncTime
        {
            get { return Math.Min(Left.SyncTime, Right.SyncTime); }
        }
    }
}