namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FSubWindow<T> : FSubWindowable<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public T[] Data;

        /// <summary>
        /// 
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public virtual T this[int i] => Data[i];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="data"></param>
        public FSubWindow(int length, T[] data)
        {
            Length = length;
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        public FSubWindow(int length) : this(length, new T[length])
        {
        }
    }
}