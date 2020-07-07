using Microsoft.StreamProcessing.Internal;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public class OutputFWindow<TPayload, TResult> : UnaryFWindow<TResult, TResult>
    {
        private StreamMessage<Empty, TResult> _obatch;
        private bool _dirPayload;
        private bool _dirSync;
        private bool _dirOther;
        private bool _dirBV;

        /// <summary>
        /// 
        /// </summary>
        public OutputFWindow(FWindowable<TResult> fwindow, FWindowable<TPayload> iwindow)
            : base(fwindow, fwindow.Size, fwindow.Period, fwindow.Offset)
        {
            _dirPayload = ((typeof(TPayload) == typeof(TResult)
                            && (iwindow.Payload.Data as TResult[] == fwindow.Payload.Data)));
            _dirSync = (iwindow.Sync.Data == fwindow.Sync.Data);
            _dirOther = (iwindow.Other.Data == fwindow.Other.Data);
            _dirBV = (iwindow.BV.Data == fwindow.BV.Data);
        }

        private static void UpdateSubWindows<B, T>(FSubWindowable<B, T> fsubwin, ColumnBatch<B> output, int offset
        )
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

        /// <summary>
        /// 
        /// </summary>
        public void SetBatch(StreamMessage<Empty, TResult> obatch)
        {
            _obatch = obatch;
            UpdateSubWindows(Input.Payload, obatch.payload, obatch.Count);
            UpdateSubWindows(Input.Sync, obatch.vsync, obatch.Count);
            UpdateSubWindows(Input.Other, obatch.vother, obatch.Count);
            UpdateSubWindows(Input.BV, obatch.bitvector, obatch.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override int _Compute()
        {
            var len = Input.Compute();
            _obatch.Count += len;
            if (Input.Payload.isOutput) Input.Payload.Offset = _obatch.Count;
            if (Input.Sync.isOutput) Input.Sync.Offset = _obatch.Count;
            if (Input.Other.isOutput) Input.Other.Offset = _obatch.Count;
            if (Input.BV.isOutput) Input.BV.Offset = _obatch.Count;
            return len;
        }
    }
}