namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class MulticastFWindow<TPayload> : UnaryFWindow<TPayload, TPayload>
    {
        private bool _isComputed;
        private int _len;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public MulticastFWindow(FWindowable<TPayload> input) : base(input, input.Size, input.Period, input.Offset)
        {
            _isComputed = false;
            _len = -1;
            _Payload = Input.Payload as FSubWindow<TPayload>;
            _Sync = Input.Sync as FSubWindow<long>;
            _Other = Input.Other as FSubWindow<long>;
            _BV = Input.BV as BVFSubWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override int _Compute()
        {
            if (!_isComputed)
            {
                _len = Input.Compute();
                _isComputed = true;
            }

            return _len;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool _Slide()
        {
            _isComputed = false;
            _len = -1;
            return Input.Slide();
        }
    }
}