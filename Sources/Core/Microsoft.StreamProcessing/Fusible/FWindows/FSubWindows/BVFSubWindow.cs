namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public class BVFSubWindow : FSubWindowable<long, bool>
    {
        /// <summary>
        /// 
        /// </summary>
        public long[] Data { get; set; }

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
        public virtual bool this[int i] => ((Data[(Offset + i) >> 6] & (1L << ((Offset + i) & 0x3f))) == 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="offset"></param>
        /// <param name="data"></param>
        public BVFSubWindow(int length, int offset, long[] data)
        {
            Length = length;
            Offset = offset;
            Data = data;
            for (int i = Length; i < data.Length * (1 << 6); i++)
            {
                Data[i >> 6] |= (1L << (i & 0x3f));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        public BVFSubWindow(int length) : this(length, 0, new long[(length >> 6) + 1])
        {
        }
    }
}