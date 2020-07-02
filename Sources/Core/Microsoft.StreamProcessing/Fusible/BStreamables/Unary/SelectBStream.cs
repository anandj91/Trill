using System;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class SelectBStream<TPayload, TResult> : UnaryBStream<TPayload, TResult>
    {
        private Func<TPayload, TResult> Selector;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="selector"></param>
        public SelectBStream(BStreamable<TPayload> stream, Func<TPayload, TResult> selector)
            : base(stream, stream.Period, stream.Offset)
        {
            Selector = selector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TResult GetPayload() => Selector(Stream.GetPayload());
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetSyncTime() => Stream.GetSyncTime();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override long GetOtherTime() => Stream.GetOtherTime();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool GetBV() => Stream.GetBV();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHash() => Stream.GetHash();
        /// <summary>
        /// 
        /// </summary>
        public override void Next() => Stream.Next();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override BStreamable<TResult> Clone()
        {
            return new SelectBStream<TPayload, TResult>(Stream, Selector);
        }
    }
}