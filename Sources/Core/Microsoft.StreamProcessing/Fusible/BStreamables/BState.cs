namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BState
    {
        private bool _ready;

        /// <summary>
        /// 
        /// </summary>
        public bool Ready
        {
            get { return _ready; }
            set { _ready = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected BState()
        {
            _ready = false;
        }
    }
}