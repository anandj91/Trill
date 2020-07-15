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
        private bool _hasSlid;
        private bool _slide;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public MulticastFWindow(FWindowable<TPayload> input)
            : base(input, input.Size, input.Period, input.Offset, input.Duration)
        {
            _isComputed = false;
            _len = -1;
            _hasSlid = false;
            _slide = false;
            _Payload = Input.Payload as FSubWindow<TPayload>;
            _Sync = Input.Sync as FSubWindow<long>;
            _Other = Input.Other as FSubWindow<long>;
            _BV = Input.BV as BVFSubWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override bool _Init() => Input.Init();

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
                _hasSlid = false;
                _slide = false;
                SyncTime = Input.SyncTime;
            }

            return _len;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool _Slide(long tsync)
        {
            if (!_hasSlid)
            {
                _slide = Input.Slide(tsync);
                _hasSlid = true;
                _isComputed = false;
                _len = -1;
            }

            return _slide;
        }
    }
}