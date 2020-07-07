using Microsoft.StreamProcessing.Internal;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public class OutputFWindow<TResult> : UnaryFWindow<TResult, TResult>
    {
        private StreamMessage<Empty, TResult> _obatch;

        /// <summary>
        /// 
        /// </summary>
        public OutputFWindow(FWindowable<TResult> fwindow) : base(fwindow, fwindow.Size, fwindow.Period, fwindow.Offset)
        {
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
            return len;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool _Slide()
        {
            if (Input.Payload.isOutput) Input.Payload.Offset = _obatch.Count;
            if (Input.Sync.isOutput) Input.Sync.Offset = _obatch.Count;
            if (Input.Other.isOutput) Input.Other.Offset = _obatch.Count;
            if (Input.BV.isOutput) Input.BV.Offset = _obatch.Count;
            return Input.Slide();
        }
    }
}