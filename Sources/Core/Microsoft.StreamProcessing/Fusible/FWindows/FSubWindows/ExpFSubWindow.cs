using System;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExpFSubWindow<T> : FSubWindowable<T>
    {
        private Func<int, T> _fn;

        /// <summary>
        /// 
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public T this[int i] => _fn(i);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="fn"></param>
        public ExpFSubWindow(int length, Func<int, T> fn)
        {
            Length = length;
            _fn = fn;
        }
    }
}