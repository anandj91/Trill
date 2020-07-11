namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class FInputOperation<TPayload> : FOperation<TPayload>
    {
        private InputFWindow<TPayload> _input;
        private readonly long _period;
        private readonly long _offset;

        /// <summary>
        /// 
        /// </summary>
        public FInputOperation(long period, long offset)
        {
            _period = period;
            _offset = offset;
        }

        /// <summary>
        /// 
        /// </summary>
        public long Size
        {
            get { return Period; }
        }

        /// <summary>
        /// 
        /// </summary>
        public long Period
        {
            get { return _period; }
        }

        /// <summary>
        /// 
        /// </summary>
        public long Offset
        {
            get { return _offset; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FWindowable<TPayload> Compile(long offset, long size)
        {
            _input = new InputFWindow<TPayload>(size, Period, Offset);
            return _input;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public InputFWindow<TPayload> GetInputFWindow()
        {
            return _input;
        }
    }
}