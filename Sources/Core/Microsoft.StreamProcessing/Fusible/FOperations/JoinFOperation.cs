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
    public class JoinFOperation<TLeft, TRight, TResult> : BinaryFOperation<TLeft, TRight, TResult>
    {
        private Expression<Func<TLeft, TRight, TResult>> _joiner;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="joiner"></param>
        public JoinFOperation(FOperation<TLeft> left, FOperation<TRight> right,
            Expression<Func<TLeft, TRight, TResult>> joiner
        ) : base(left, right)
        {
            _joiner = joiner;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="dryRun"></param>
        /// <returns></returns>
        public override FWindowable<TResult> Compile(int factor, bool dryRun = false)
        {
            var lwin = Left.Compile(1, true);
            var rwin = Right.Compile(1, true);
            Invariant.IsTrue(rwin.Period % lwin.Period == 0, "Right period must be a multiple of left period");

            return new JoinFWindow<TLeft, TRight, TResult>(
                Left.Compile((int) (factor * rwin.Size / lwin.Size), dryRun),
                Right.Compile(factor, dryRun),
                _joiner
            );
        }
    }
}