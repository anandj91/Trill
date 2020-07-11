namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class UnaryFWindow<TPayload, TResult> : FWindow<TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        protected FWindowable<TPayload> Input;

        /// <summary>
        /// 
        /// </summary>
        public UnaryFWindow(FWindowable<TPayload> input, long size, long period, long offset)
            : base(size, period, offset)
        {
            Input = input;
        }

        /// <summary>
        /// 
        /// </summary>
        public override long SyncTime
        {
            get { return Input.SyncTime; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool _Slide(long tsync) => Input.Slide(tsync);
    }
}