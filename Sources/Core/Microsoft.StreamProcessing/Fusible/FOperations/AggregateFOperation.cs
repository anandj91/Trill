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
            _aggregate = aggregate;
            _window = window;
            _period = period;
            _offset = offset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override FWindowable<TResult> Compile(int factor, bool dryRun = false)
        {
            var tmp = Input.Compile(1, true);
            var iperiod = tmp.Period;
            var ifactor = (int) (_window / iperiod) / tmp.Length;
            return new AggregateFWindow<TPayload, TAggState, TResult>(
                Input.Compile(ifactor * factor, dryRun), _aggregate, _window
            );
        }
    }
}