using System;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public class WhereBStream<TPayload> : UnaryBStream<TPayload, UnaryBState, TPayload>
    {
        private Func<TPayload, bool> Filter;

        /// <summary>
        /// 
        /// </summary>
        public WhereBStream(BStreamable<TPayload> stream, Func<TPayload, bool> filter)
            : base(stream, stream.Period, stream.Offset)
        {
            Filter = filter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override TPayload Selector(TPayload payload) => payload;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool ProcessNextItem(UnaryBState state)
        {
            return Filter(Stream.GetPayload(state.i));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override UnaryBState _Init() => new UnaryBState(Stream.Init());
    }
}