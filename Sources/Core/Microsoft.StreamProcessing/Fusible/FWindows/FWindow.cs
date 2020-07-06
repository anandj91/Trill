using System;
using System.Linq.Expressions;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public abstract class FWindow<TPayload> : FWindowable<TPayload>
    {
        /// <summary>
        /// 
        /// </summary>
        public long Size { get; }

        /// <summary>
        /// 
        /// </summary>
        public long Period { get; }

        /// <summary>
        /// 
        /// </summary>
        public long Offset { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 
        /// </summary>
        public FWindow(long size, long period, long offset)
        {
            Size = size;
            Period = period;
            Offset = offset;
            Length = (int) (Size / Period);
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract FSubWindowable<TPayload> Payload { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract FSubWindowable<long> Sync { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract FSubWindowable<long> Other { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract FSubWindowable<bool> BV { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract long GetInputSize();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Expression<Func<long, long>> GetInputSync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Expression<Func<long, long>> GetOutputSync();

        /// <summary>
        /// 
        /// </summary>
        public int Compute() => _Compute();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract int _Compute();
    }
}