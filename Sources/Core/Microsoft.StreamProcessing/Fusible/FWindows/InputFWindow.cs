using System;
using System.Linq.Expressions;

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

        private OffsetFSubWindow<TPayload> _payload;
        private OffsetFSubWindow<long> _sync;
        private OffsetFSubWindow<long> _other;
        private OffsetFSubWindow<bool> _bv;

        /// <summary>
        /// 
        /// </summary>
        public InputFWindow(long size, long period, long offset) : base(size, period, offset)
        {
            _payload = new OffsetFSubWindow<TPayload>(Length, new FSubWindow<TPayload>(Length));
            _sync = new OffsetFSubWindow<long>(Length, new FSubWindow<long>(Length));
            _other = new OffsetFSubWindow<long>(Length, new FSubWindow<long>(Length));
            _bv = new OffsetFSubWindow<bool>(Length, new BVFSubWindow(Length));
        }

        /// <summary>
        /// 
        /// </summary>
        public override FSubWindowable<TPayload> Payload
        {
            get { return _payload; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override FSubWindowable<long> Sync
        {
            get { return _sync; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override FSubWindowable<long> Other
        {
            get { return _other; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override FSubWindowable<bool> BV
        {
            get { return _bv; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetInputSize() => Size;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<long, long>> GetInputSync() => (t) => t;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<long, long>> GetOutputSync() => (t) => t;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Slide()
        {
            //TODO: Need to deal with gaps
            Idx += Length;
            ((OffsetFSubWindow<TPayload>) Payload).Offset = Idx;
            ((OffsetFSubWindow<long>) Sync).Offset = Idx;
            ((OffsetFSubWindow<long>) Other).Offset = Idx;
            ((OffsetFSubWindow<bool>) BV).Offset = Idx;
            return Idx < Count;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int _Compute() => Math.Min(Length, Count - Idx);

        /// <summary>
        /// 
        /// </summary>
        public void SetBatch(StreamMessage<Empty, TPayload> batch)
        {
            Idx = 0;
            Count = batch.Count;
            //TODO: Need to deal with gaps
            ((FSubWindow<TPayload>) _payload.Base).Data = batch.payload.col;
            _payload.Offset = 0;
            ((FSubWindow<long>) _sync.Base).Data = batch.vsync.col;
            _sync.Offset = 0;
            ((FSubWindow<long>) _other.Base).Data = batch.vother.col;
            _other.Offset = 0;
            ((BVFSubWindow) _bv.Base).BV = batch.bitvector.col;
            _bv.Offset = 0;
        }
    }
}