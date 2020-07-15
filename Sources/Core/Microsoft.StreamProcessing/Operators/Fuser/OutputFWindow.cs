using Microsoft.StreamProcessing.Internal;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public class OutputFWindow<TResult> : FWindowable<TResult>
    {
        private StreamMessage<Empty, TResult> _obatch;
        private bool isUpdated;
        private FWindowable<TResult> _fwindow;

        /// <summary>
        /// 
        /// </summary>
        public OutputFWindow(FWindowable<TResult> fwindow)
        {
            _fwindow = fwindow;
            isUpdated = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public long SyncTime
        {
            get { return _fwindow.SyncTime; }
        }

        /// <summary>
        /// 
        /// </summary>
        public long Duration
        {
            get { return _fwindow.Duration; }
        }

        /// <summary>
        /// 
        /// </summary>
        public long Size
        {
            get { return _fwindow.Size; }
        }

        /// <summary>
        /// 
        /// </summary>
        public long Period
        {
            get { return _fwindow.Period; }
        }

        /// <summary>
        /// 
        /// </summary>
        public long Offset
        {
            get { return _fwindow.Offset; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Length
        {
            get { return _fwindow.Length; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FSubWindowable<TResult, TResult> Payload
        {
            get { return _fwindow.Payload; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FSubWindowable<long, long> Sync
        {
            get { return _fwindow.Sync; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FSubWindowable<long, long> Other
        {
            get { return _fwindow.Other; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FSubWindowable<long, bool> BV
        {
            get { return _fwindow.BV; }
        }

        private static void UpdateSubWindows<B, T>(FSubWindowable<B, T> fsubwin, ColumnBatch<B> output, int offset)
        {
            if (fsubwin.isInput)
            {
                output.col = fsubwin.Data;
            }
            else
            {
                fsubwin.Data = output.col;
                fsubwin.Offset = offset;
                fsubwin.isOutput = true;
            }
        }

        private void CheckAndUpdate()
        {
            if (!isUpdated)
            {
                UpdateSubWindows(Payload, _obatch.payload, _obatch.Count);
                UpdateSubWindows(Sync, _obatch.vsync, _obatch.Count);
                UpdateSubWindows(Other, _obatch.vother, _obatch.Count);
                UpdateSubWindows(BV, _obatch.bitvector, _obatch.Count);
                isUpdated = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetBatch(StreamMessage<Empty, TResult> obatch)
        {
            _obatch = obatch;
            isUpdated = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            var ret = _fwindow.Init();
            CheckAndUpdate();
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Compute()
        {
            CheckAndUpdate();
            var len = _fwindow.Compute();
            _obatch.Count += len;
            return len;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Slide(long tsync)
        {
            if (Payload.isOutput) Payload.Offset = _obatch.Count;
            if (Sync.isOutput) Sync.Offset = _obatch.Count;
            if (Other.isOutput) Other.Offset = _obatch.Count;
            if (BV.isOutput) BV.Offset = _obatch.Count;
            return _fwindow.Slide(tsync);
        }
    }
}