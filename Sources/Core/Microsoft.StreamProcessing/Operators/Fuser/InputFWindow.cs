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
        public InputFWindow(long size, long period, long offset) : base(size, period, offset)
        {
            Payload.isInput = true;
            Sync.isInput = true;
            Other.isInput = true;
            BV.isInput = true;
            _syncTime = StreamEvent.MinSyncTime;
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

            return s + len;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool _Slide(long tsync)
        {
            if (tsync < SyncTime)
            {
                throw new Exception("Sliding to an already synced time");
            }

            var shift = NextShift(tsync + Period);
            if (Idx + shift >= Count)
            {
                return false;
            }

            Idx += shift;
            Payload.Offset = Idx;
            Sync.Offset = Idx;
            Other.Offset = Idx;
            BV.Offset = Idx;
            _syncTime = Sync[0];
            return SyncTime < StreamEvent.InfinitySyncTime;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int _Compute()
        {
            var len = NextShift(SyncTime + Size);
            // TODO: Deal with gaps
            _syncTime = Sync[len - 1];
            return Math.Min(len, Count - Idx);
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
            _syncTime = Sync[0];
        }
    }
}