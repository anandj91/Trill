using System;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class InputFWindow<TPayload> : FWindow<TPayload>
    {
        private int Idx;
        private int Count;
        private long _syncTime;

        /// <summary>
        /// 
        /// </summary>
        public InputFWindow(long size, long period, long offset) : base(size, period, offset, period)
        {
            Payload.isInput = true;
            Sync.isInput = true;
            Other.isInput = true;
            BV.isInput = true;
            _syncTime = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        public override long SyncTime
        {
            get { return _syncTime; }
        }

        private int NextShift(long tsync)
        {
            if (Sync[0] == StreamEvent.InfinitySyncTime) return 1;

            int len = (int) Math.Min((tsync - Sync[0]) / Period, Count - Idx);
            int s = 0;
            while (len > 0 && Sync[s + len - 1] > tsync)
            {
                len /= 2;
                if (Sync[s + len - 1] < tsync)
                {
                    s += len;
                }
            }

            if (len < 0)
            {
                len = 0;
            }

            return s + len;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool _Slide(long tsync)
        {
            tsync = Math.Max((tsync / Size) * Size, _syncTime);
            var shift = NextShift(tsync);
            if (Idx + shift >= Count)
            {
                return false;
            }

            Idx += shift;
            Payload.Offset = Idx;
            Sync.Offset = Idx;
            Other.Offset = Idx;
            BV.Offset = Idx;
            _syncTime = Sync.Data[Idx];
            return SyncTime < StreamEvent.InfinitySyncTime;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int _Compute()
        {
            var len = Math.Min(Length, Count - Idx);
            _syncTime += Size;
            return len;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetBatch(StreamMessage<Empty, TPayload> batch)
        {
            Idx = 0;
            Count = batch.Count;
            //TODO: Need to deal with gaps
            _Payload.Data = batch.payload.col;
            _Payload.Offset = 0;
            _Sync.Data = batch.vsync.col;
            _Sync.Offset = 0;
            _Other.Data = batch.vother.col;
            _Other.Offset = 0;
            _BV.Data = batch.bitvector.col;
            _BV.Offset = 0;
        }
    }
}