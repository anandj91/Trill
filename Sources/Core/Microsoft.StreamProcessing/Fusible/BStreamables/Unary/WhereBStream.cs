using System;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class WhereBStream<TPayload> : UnaryBStream<TPayload, TPayload>
    {
        private Func<TPayload, bool> Filter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filter"></param>
        /// <param name="period"></param>
        /// <param name="offset"></param>
        public WhereBStream(BStreamable<TPayload> stream, Func<TPayload, bool> filter, long period, long offset)
            : base(stream, period, offset)
        {
            Filter = filter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TPayload GetPayload() => Stream.GetPayload();

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
        public override bool GetBV() => true;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHash() => Stream.GetHash();

        /// <summary>
        /// 
        /// </summary>
        public override void Next()
        {
            do
            {
                if (IsDone()) return;
                Stream.Next();
            } while (!Stream.GetBV() || Filter(Stream.GetPayload()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override BStreamable<TPayload> Clone()
        {
            return new WhereBStream<TPayload>(Stream, Filter, Period, Offset);
        }
    }
}