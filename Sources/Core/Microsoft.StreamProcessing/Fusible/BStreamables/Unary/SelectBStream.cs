using System;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectBStream<TInput, TOutput> : UnaryBStream<TInput, UnaryBState, TOutput>
    {
        private Func<TInput, TOutput> Select;

        /// <summary>
        /// 
        /// </summary>
        public SelectBStream(BStreamable<TInput> stream, Func<TInput, TOutput> select)
            : base(stream, stream.Period, stream.Offset)
        {
            Select = select;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override TOutput Selector(TInput payload)
        {
            return Select(payload);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool ProcessNextItem(UnaryBState state) => true;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override UnaryBState _Init() => new UnaryBState(Stream.Init());
    }
}