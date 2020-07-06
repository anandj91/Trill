namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FSubWindow<T> : FSubWindowable<T, T>
    {
        /// <summary>
        /// 
        /// </summary>
        public T[] Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public virtual T this[int i] => Data[Offset + i];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        public FSubWindow(int length, int offset, T[] data)
        {
            Length = length;
            Data = data;
            Offset = offset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        public FSubWindow(int length) : this(length, 0, new T[length])
        {
        }
    }
}