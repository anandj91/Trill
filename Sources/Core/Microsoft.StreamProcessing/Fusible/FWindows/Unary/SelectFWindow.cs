using System;
using System.Linq.Expressions;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class SelectFWindow<TPayload, TResult> : UnaryFWindow<TPayload, TResult>
    {
        private Func<TPayload, TResult> _selector;

        /// <summary>
        /// 
        /// </summary>
        public SelectFWindow(FWindowable<TPayload> input, Expression<Func<TPayload, TResult>> selector)
            : base(input, input.Size, input.Period, input.Offset)
        {
            _selector = selector.Compile();
            _Payload = new FSubWindow<TResult>(Length);
            _Sync = Input.Sync as FSubWindow<long>;
            _Other = Input.Other as FSubWindow<long>;
            _BV = Input.BV as BVFSubWindow;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override int _Compute()
        {
            var len = Input.Compute();

            var payload = Input.Payload.Data;
            var payloadOffset = Input.Payload.Offset;
            var bvOffset = Input.BV.Offset;

            unsafe
            {
                fixed (long* bv = Input.BV.Data)
                {
                    for (int i = 0; i < len; i++)
                    {
                        // TODO: Flush at the end of stream
                        var bi = bvOffset + i;
                        if ((bv[bi >> 6] & (1L << (bi & 0x3f))) == 0)
                        {
                            var pi = payloadOffset + i;
                            _Payload.Data[i] = _selector(payload[pi]);
                        }
                    }
                }
            }

            return len;
        }
    }
}