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
        public int Size { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public virtual T this[int i] => Data[i];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="data"></param>
        public FSubWindow(int size, T[] data)
        {
            Size = size;
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public FSubWindow(int size) : this(size, new T[size])
        {
        }
    }
}