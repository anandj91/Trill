using System;
using System.Collections.Concurrent;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class InputFWindow<TPayload> : FWindow<TPayload>
    {
        private BlockingCollection<StreamMessage<Empty, TPayload>> _queue;
        private StreamMessage<Empty, TPayload> _batch;
        private int Idx;
        private int Count;
        private long _syncTime;

        /// <summary>
        /// 
        /// </summary>
        public InputFWindow(BlockingCollection<StreamMessage<Empty, TPayload>> queue,
            long size, long period, long offset
        ) : base(size, period, offset, period)
        {
            _queue = queue;
            Payload.isInput = true;
            Sync.isInput = true;
            Other.isInput = true;
            BV.isInput = true;
            _syncTime = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        ~InputFWindow()
        {
            CheckAndRelease();
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
        protected override bool _Init()
        {
            try
            {
                if (_batch == null)
                {
                    var batch = _queue.Take();
                    SetBatch(batch);
                    _syncTime = Sync[0];
                }
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            return true;
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
        /// <param name="tsync"></param>
        /// <returns></returns>
        public override bool Slide(long tsync)
        {
            while (!_Slide(tsync))
            {
                try
                {
                    var batch = _queue.Take();
                    SetBatch(batch);
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }

            return true;
        }

        private void CheckAndRelease()
        {
            if (_batch != null)
            {
                _batch.Release();
                _batch.Return();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetBatch(StreamMessage<Empty, TPayload> batch)
        {
            CheckAndRelease();
            _batch = batch;
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