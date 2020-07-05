using System;
using System.Linq.Expressions;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FSubInputWindow<T> : FSubWindow<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public int Offset;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public override T this[int i] => base[Offset + i];

        /// <summary>
        /// 
        /// </summary>
        public FSubInputWindow(int size, T[] data) : base(size, data)
        {
            Offset = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public FSubInputWindow(int size) : base(size)
        {
            Offset = 0;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class InputFWindow<TPayload> : FWindow<TPayload>
    {
        private int Idx;
        private int Count;

        private FSubInputWindow<TPayload> _payload;
        private FSubInputWindow<long> _sync;
        private FSubInputWindow<long> _other;
        private FSubInputWindow<bool> _bv;

        /// <summary>
        /// 
        /// </summary>
        public InputFWindow(long size, long period, long offset) : base(size, period, offset)
        {
            var s = Length;
            _payload = new FSubInputWindow<TPayload>(s);
            _sync = new FSubInputWindow<long>(s);
            _other = new FSubInputWindow<long>(s);
            _bv = new FSubInputWindow<bool>(s);
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
            //TODO: Need to deal with gaps and bitvector
            Idx += Length;
            ((FSubInputWindow<TPayload>) Payload).Offset = Idx;
            ((FSubInputWindow<long>) Sync).Offset = Idx;
            ((FSubInputWindow<long>) Other).Offset = Idx;
            return Idx < Count;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void _Compute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetBatch(StreamMessage<Empty, TPayload> batch)
        {
            Idx = 0;
            Count = batch.Count;
            //TODO: Need to deal with gaps and bitvector
            var s = Length;
            _payload.Data = batch.payload.col;
            _payload.Offset = 0;
            _sync.Data = batch.vsync.col;
            _sync.Offset = 0;
            _other.Data = batch.vother.col;
            _other.Offset = 0;
            _bv.Data = new bool[Count];
            _bv.Offset = 0;
            for (int i = 0; i < Count; i++)
            {
                if ((batch.bitvector.col[i >> 6] & (1L << (i & 0x3f))) == 0) _bv.Data[i] = true;
            }
        }
    }
}