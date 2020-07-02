namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public abstract class UnaryBStream<TPayload, TResult> : BStream<TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        protected BStreamable<TPayload> Stream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        public UnaryBStream(BStreamable<TPayload> stream, long period, long offset) : base(period, offset)
        {
            Stream = stream;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsDone() => Stream.IsDone();

        /// <summary>
        /// 
        /// </summary>
        public override void Init() => Stream.Init();
    }
}