namespace Microsoft.StreamProcessing
{
    internal class BufferWindow<TPayload> : FWindow<TPayload>
    {
        public BufferWindow(long size, long period, long offset, long duration) : base(size, period, offset, duration)
        {
        }

        protected override bool _Init()
        {
            throw new System.NotImplementedException();
        }

        protected override int _Compute()
        {
            throw new System.NotImplementedException();
        }

        protected override bool _Slide(long tsync)
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class BufferedFWindow<TPayload> : UnaryFWindow<TPayload, TPayload>
    {
        private int _count;
        private int _pos;
        private BufferWindow<TPayload>[] _buffer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="count"></param>
        public BufferedFWindow(FWindowable<TPayload> input, int count)
            : base(input, input.Size, input.Period, input.Offset, input.Duration)
        {
            _count = count;
            _pos = 0;
            _buffer = new BufferWindow<TPayload>[_count];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool _Init()
        {
            var ret = Input.Init();
            int i = 0;
            while (ret && i < _count - 1)
            {
                _buffer[_pos].SyncTime = Input.SyncTime;
                _Compute();
                ret &= _Slide(Input.SyncTime);
                i++;
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override int _Compute()
        {
            var len = Input.Compute();
            Input.Payload.Copy(_buffer[_pos].Payload);
            Input.Sync.Copy(_buffer[_pos].Sync);
            Input.Other.Copy(_buffer[_pos].Other);
            Input.BV.Copy(_buffer[_pos].BV);
            _pos = (_pos + 1) % _count;
            _Payload = _buffer[_pos].Payload as FSubWindow<TPayload>;
            _Sync = _buffer[_pos].Sync as FSubWindow<long>;
            _Other = _buffer[_pos].Other as FSubWindow<long>;
            _BV = _buffer[_pos].BV as BVFSubWindow;
            SyncTime = _buffer[_pos].SyncTime;
            return len;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tsync"></param>
        /// <returns></returns>
        protected override bool _Slide(long tsync) => Input.Slide(tsync);
    }
}