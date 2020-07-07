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

        /// <summary>
        /// 
        /// </summary>
        public InputFWindow(long size, long period, long offset) : base(size, period, offset)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Slide()
        {
            //TODO: Need to deal with gaps
            Idx += Length;
            Payload.Offset = Idx;
            Sync.Offset = Idx;
            Other.Offset = Idx;
            BV.Offset = Idx;
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