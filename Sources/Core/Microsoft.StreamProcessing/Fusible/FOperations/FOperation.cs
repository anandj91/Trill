namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public interface FOperation<TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FWindowable<TResult> Compile(int i);
    }
}