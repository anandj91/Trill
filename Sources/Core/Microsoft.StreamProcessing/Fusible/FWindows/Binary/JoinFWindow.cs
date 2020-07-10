using System;
using System.Linq.Expressions;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class JoinFWindow<TLeft, TRight, TResult> : BinaryFWindow<TLeft, TRight, TResult>
    {
        private Func<TLeft, TRight, TResult> _joiner;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="joiner"></param>
        public JoinFWindow(FWindowable<TLeft> left, FWindowable<TRight> right,
            Expression<Func<TLeft, TRight, TResult>> joiner
        ) : base(left, right, left.Size, left.Period, left.Offset)
        {
            Invariant.IsTrue(right.Offset == left.Offset, "Left offset must match to right offset");
            Invariant.IsTrue(right.Period % left.Period == 0, "Right period must be a multiple of left period");
            Invariant.IsTrue(right.Size == left.Size, "Left size must match to right size");

            _joiner = joiner.Compile();

            _Payload = new FSubWindow<TResult>(Length);
            _Sync = Left.Sync as FSubWindow<long>;
            _Other = Left.Other as FSubWindow<long>;
            _BV = new BVFSubWindow(Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override int _Compute()
        {
            var llen = Left.Compute();
            var rlen = Right.Compute();
            int f;
            if (llen == 0 || rlen == 0)
            {
                f = 0;
            }
            else
            {
                f = llen / rlen;
            }
            

            var lpayload = Left.Payload.Data;
            var lpayloadOffset = Left.Payload.Offset;
            var rpayload = Right.Payload.Data;
            var rpayloadOffset = Right.Payload.Offset;
            var opayload = Payload.Data;
            var opayloadOffset = Payload.Offset;

            var lbvOffset = Left.BV.Offset;
            var rbvOffset = Right.BV.Offset;
            var obvOffset = BV.Offset;

            unsafe
            {
                fixed (long* lbv = Left.BV.Data)
                fixed (long* rbv = Right.BV.Data)
                fixed (long* obv = BV.Data)
                {
                    for (int r = 0; r < rlen; r++)
                    {
                        var rbi = rbvOffset + r;
                        if ((rbv[rbi >> 6] & (1L << (rbi & 0x3f))) == 0)
                        {
                            for (int l = r * f; l < (r + 1) * f; l++)
                            {
                                var lbi = lbvOffset + l;
                                var obi = obvOffset + l;
                                if (((lbv[lbi >> 6] & (1L << (lbi & 0x3f))) == 0)
                                    && ((rbv[rbi >> 6] & (1L << (rbi & 0x3f))) == 0))
                                {
                                    var lp = lpayload[lpayloadOffset + l];
                                    var rp = rpayload[rpayloadOffset + r];
                                    var po = opayloadOffset + l;
                                    opayload[po] = _joiner(lp, rp);
                                    obv[obi >> 6] &= ~(1L << (obi & 0x3f));
                                }
                                else
                                {
                                    obv[obi >> 6] |= (1L << (obi & 0x3f));
                                }
                            }
                        }
                    }
                }
            }

            return llen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool _Slide() => Left.Slide() && Right.Slide();
    }
}