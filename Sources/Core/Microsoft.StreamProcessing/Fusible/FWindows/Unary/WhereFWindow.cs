using System;
using System.Linq.Expressions;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class WhereFWindow<TPayload> : UnaryFWindow<TPayload, TPayload>
    {
        private Func<TPayload, bool> _filter;
        private BVFSubWindow _bv;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="filter"></param>
        public WhereFWindow(FWindowable<TPayload> input, Expression<Func<TPayload, bool>> filter)
            : base(input, input.Size, input.Period, input.Offset)
        {
            _bv = new BVFSubWindow(Length);
            _filter = filter.Compile();
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
        protected override int _Compute()
        {
            var len = Input.Compute();

            /* Init bits to zero */
            for (int i = 0; i < _bv.BV.Length; i++)
            {
                _bv.BV[i] = 0;
            }

            /* Set bits */
            for (int i = 0; i < len; i++)
            {
                if (!Input.BV[i] || !_filter(Payload[i]))
                    _bv.BV[i >> 6] |= (1L << (i & 0x3f));
            }

            /* Set residual bits */
            for (int i = len; i < _bv.BV.Length; i++)
            {
                _bv.BV[i >> 6] |= (1L << (i & 0x3f));
            }

            return len;
        }
    }
}