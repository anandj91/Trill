using System;
using System.Linq.Expressions;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InputFSubWindow<T> : FSubWindow<T>
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
        public InputFSubWindow(int size, T[] data) : base(size, data)
        {
            Offset = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public InputFSubWindow(int size) : base(size)
        {
            Offset = 0;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InputBVFSubWindow : FSubWindowable<bool>
    {
        /// <summary>
        /// 
        /// </summary>
        public long[] BV;

        /// <summary>
        /// 
        /// </summary>
        public int Offset;

        /// <summary>
        /// 
        /// </summary>
        public int Count;

        /// <summary>
        /// 
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public bool this[int i] => ((BV[(Offset + i) >> 6] & (1L << ((Offset + i) & 0x3f))) == 0);

        /// <summary>
        /// 
        /// </summary>
        public InputBVFSubWindow(int size, long[] bv, int count)
        {
            BV = bv;
            Size = size;
            Offset = 0;
            Count = count;
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

        private InputFSubWindow<TPayload> _payload;
        private InputFSubWindow<long> _sync;
        private InputFSubWindow<long> _other;
        private InputBVFSubWindow _bv;

        /// <summary>
        /// 
        /// </summary>
        public InputFWindow(long size, long period, long offset) : base(size, period, offset)
        {
            var s = Length;
            _payload = new InputFSubWindow<TPayload>(s);
            _sync = new InputFSubWindow<long>(s);
            _other = new InputFSubWindow<long>(s);
            _bv = new InputBVFSubWindow(s, default, 0);
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
            ((InputFSubWindow<TPayload>) Payload).Offset = Idx;
            ((InputFSubWindow<long>) Sync).Offset = Idx;
            ((InputFSubWindow<long>) Other).Offset = Idx;
            ((InputBVFSubWindow) BV).Offset = Idx;
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
            _payload.Data = batch.payload.col;
            _payload.Offset = 0;
            _sync.Data = batch.vsync.col;
            _sync.Offset = 0;
            _other.Data = batch.vother.col;
            _other.Offset = 0;
            _bv.BV = batch.bitvector.col;
            _bv.Offset = 0;
            _bv.Count = Count;
        }
    }
}