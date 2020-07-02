using System;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class JoinBStream<TLeft, TRight, TResult> : BinaryBStream<TLeft, TRight, TResult>
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
            BStreamable<TLeft> left,
            BStreamable<TRight> right,
            Func<TLeft, TRight, TResult> joiner,
            long period, long offset)
            : base(left, right, period, offset)
        {
            Joiner = joiner;
            Left.Next();
            Right.Next();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TResult GetPayload() => Joiner(Left.GetPayload(), Right.GetPayload());

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetSyncTime() => Math.Max(Left.GetSyncTime(), Right.GetSyncTime());

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetOtherTime() => Math.Min(Left.GetOtherTime(), Right.GetOtherTime());

        /// <summary>
        /// 
        /// </summary>
        public override void Next()
        {
            while (!Overlap())
            {
                if (Left.GetSyncTime() < Right.GetSyncTime())
                {
                    Left.Next();
                }
                else
                {
                    Right.Next();
                }
            }
        }

        private bool Overlap()
        {
            return (Math.Max(Left.GetSyncTime(), Right.GetSyncTime()) <
                    Math.Min(Left.GetOtherTime(), Right.GetOtherTime()));
        }
    }
}