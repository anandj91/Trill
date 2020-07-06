namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OffsetFSubWindow<T> : FSubWindowable<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public FSubWindowable<T> Base;

        /// <summary>
        /// 
        /// </summary>
        public int Offset;

        /// <summary>
        /// 
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public T this[int i] => Base[Offset + i];

        /// <summary>
        /// 
        /// </summary>
        public OffsetFSubWindow(int length, FSubWindowable<T> _base)
        {
            Offset = 0;
            Length = length;
            Base = _base;
        }
    }
}