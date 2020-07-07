namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class MulticastFOperation<TPayload> : UnaryFOperation<TPayload, TPayload>
    {
        private MulticastFWindow<TPayload> _iwindow;
        private bool _isCompiled;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public MulticastFOperation(FOperation<TPayload> input) : base(input)
        {
            _isCompiled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="dryRun"></param>
        /// <returns></returns>
        public override FWindowable<TPayload> Compile(int factor, bool dryRun = false)
        {
            if (dryRun)
            {
                return new MulticastFWindow<TPayload>(Input.Compile(factor, true));
            }
            else
            {
                if (!_isCompiled)
                {
                    _iwindow = new MulticastFWindow<TPayload>(Input.Compile(factor, false));
                    _isCompiled = true;
                }

                return _iwindow;
            }
        }
    }
}