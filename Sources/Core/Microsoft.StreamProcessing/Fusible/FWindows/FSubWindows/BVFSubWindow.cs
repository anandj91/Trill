namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public class BVFSubWindow : FSubWindowable<bool>
    {
        /// <summary>
        /// 
        /// </summary>
        public long[] BV;

        /// <summary>
        /// 
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public virtual bool this[int i] => ((BV[i >> 6] & (1L << (i & 0x3f))) == 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="bv"></param>
        public BVFSubWindow(int length, long[] bv)
        {
            Length = length;
            BV = bv;
            for (int i = Length; i < bv.Length * (1 << 6); i++)
            {
                BV[i >> 6] |= (1L << (i & 0x3f));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        public BVFSubWindow(int length) : this(length, new long[(length >> 6) + 1])
        {
        }
    }
}