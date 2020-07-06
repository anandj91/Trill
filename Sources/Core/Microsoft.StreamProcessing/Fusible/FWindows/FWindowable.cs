using System;
using System.Linq.Expressions;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    public interface FWindowable<TPayload>
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
        /// <returns></returns>
        public FSubWindowable<TPayload> Payload { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FSubWindowable<long> Sync { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FSubWindowable<long> Other { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FSubWindowable<bool> BV { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long GetInputSize();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Expression<Func<long, long>> GetInputSync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Expression<Func<long, long>> GetOutputSync();

        /// <summary>
        /// 
        /// </summary>
        public int Compute();
    }
}