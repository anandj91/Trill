namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public abstract class BinaryFOperation<TLeft, TRight, TResult> : FOperation<TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        protected FOperation<TLeft> Left;

        /// <summary>
        /// 
        /// </summary>
        protected FOperation<TRight> Right;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public BinaryFOperation(FOperation<TLeft> left, FOperation<TRight> right)
        {
            Left = left;
            Right = right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="dryRun"></param>
        /// <returns></returns>
        public abstract FWindowable<TResult> Compile(int factor, bool dryRun = false);
    }
}