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
        public int Size { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public bool this[int i] => true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public BVFSubWindow(int size)
        {
            Size = size;
        }
    }
}