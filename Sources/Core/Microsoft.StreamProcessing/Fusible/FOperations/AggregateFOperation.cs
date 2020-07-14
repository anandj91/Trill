using Microsoft.StreamProcessing.Aggregates;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    /// <typeparam name="TAggState"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class AggregateFOperation<TPayload, TAggState, TResult> : UnaryFOperation<TPayload, TResult>
    {
        private long _window;
        private long _period;
        private long _offset;
        private IAggregate<TPayload, TAggState, TResult> _aggregate;

        /// <summary>
        /// 
        /// </summary>
        public AggregateFOperation(
            FOperation<TPayload> input,
            IAggregate<TPayload, TAggState, TResult> aggregate,
            long window, long period, long offset
        ) : base(input)
        {
            Invariant.IsTrue(window == period, "Window must be equal to period");
            _aggregate = aggregate;
            _window = window;
            _period = period;
            _offset = offset;
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Size
        {
            get { return Utility.LCM(Input.Size, _window); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Period
        {
            get { return _period; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Offset
        {
            get { return _offset; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override FWindowable<TResult> Compile(long offset, long size)
        {
            return new TumblingAggregateFWindow<TPayload, TAggState, TResult>(Input.Compile(offset, size), _aggregate, _window);
        }
    }
}