using System;
using System.Linq.Expressions;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class WhereBVFSubWindow<TPayload> : FSubWindowable<bool>
    {
        private FSubWindowable<TPayload> _payload;
        private FSubWindowable<bool> _bv;
        private Func<TPayload, bool> _filter;

        /// <summary>
        /// 
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public bool this[int i] => _bv[i] && _payload[i] != null && _filter(_payload[i]);

        /// <summary>
        /// 
        /// </summary>
        public WhereBVFSubWindow(int size, FSubWindowable<bool> bv, FSubWindowable<TPayload> payload,
            Func<TPayload, bool> filter)
        {
            Size = size;
            _payload = payload;
            _bv = bv;
            _filter = filter;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class WhereFWindow<TPayload> : UnaryFWindow<TPayload, TPayload>
    {
        private WhereBVFSubWindow<TPayload> _bv;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="filter"></param>
        public WhereFWindow(FWindowable<TPayload> input, Expression<Func<TPayload, bool>> filter)
            : base(input, input.Size, input.Period, input.Offset)
        {
            _bv = new WhereBVFSubWindow<TPayload>(Input.BV.Size, Input.BV, Input.Payload, filter.Compile());
        }

        /// <summary>
        /// 
        /// </summary>
        public override FSubWindowable<TPayload> Payload
        {
            get { return Input.Payload; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override FSubWindowable<long> Sync
        {
            get { return Input.Sync; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override FSubWindowable<long> Other
        {
            get { return Input.Other; }
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
        public override long GetInputSize() => Input.GetInputSize();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<long, long>> GetInputSync() => Input.GetInputSync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Expression<Func<long, long>> GetOutputSync() => Input.GetOutputSync();

        /// <summary>
        /// 
        /// </summary>
        protected override void _Compute() => Input.Compute();
    }
}