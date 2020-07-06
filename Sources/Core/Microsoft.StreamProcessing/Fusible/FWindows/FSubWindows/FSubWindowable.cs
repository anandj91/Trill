namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public interface FSubWindowable<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 
        /// </summary>
        public T this[int i] { get; }
    }
}