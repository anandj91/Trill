namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public abstract class BinaryBStream<TLeft, TRight, TResult> : BStream<TResult>
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
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        public BinaryBStream(BStreamable<TLeft> left, BStreamable<TRight> right, long period, long offset)
            : base(period, offset)
        {
            Left = left;
            Right = right;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool GetBV() => true;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHash() => 0;
    }
}