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
        /// <returns></returns>
        public FWindowable<TPayload> Compile(int factor)
        {
            _input = new InputFWindow<TPayload>(factor * _period, _period, _offset);
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