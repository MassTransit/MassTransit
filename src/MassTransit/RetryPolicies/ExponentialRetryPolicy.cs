namespace MassTransit.RetryPolicies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class ExponentialRetryPolicy :
        IRetryPolicy
    {
        readonly IExceptionFilter _filter;
        readonly int _highInterval;
        readonly TimeSpan[] _intervals;
        readonly int _lowInterval;
        readonly int _maxInterval;
        readonly int _minInterval;

        public ExponentialRetryPolicy(IExceptionFilter filter, int retryLimit, TimeSpan minInterval, TimeSpan maxInterval, TimeSpan intervalDelta)
        {
            _filter = filter;
            RetryLimit = retryLimit;
            _minInterval = (int)minInterval.TotalMilliseconds;
            _maxInterval = (int)maxInterval.TotalMilliseconds;

            _lowInterval = (int)(intervalDelta.TotalMilliseconds * 0.8);
            _highInterval = (int)(intervalDelta.TotalMilliseconds * 1.2);

            _intervals = CalculateIntervals().ToArray();
        }

        public int RetryLimit { get; }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Set(new
            {
                Policy = "Exponential",
                Limit = RetryLimit,
                Min = _minInterval,
                Max = _maxInterval,
                Low = _lowInterval,
                High = _highInterval
            });

            _filter.Probe(context);
        }

        RetryPolicyContext<T> IRetryPolicy.CreatePolicyContext<T>(T context)
        {
            return new ExponentialRetryPolicyContext<T>(this, context);
        }

        public bool IsHandled(Exception exception)
        {
            return _filter.Match(exception);
        }

        public TimeSpan GetRetryInterval(int retryCount)
        {
            return retryCount < _intervals.Length ? _intervals[retryCount] : _intervals[_intervals.Length - 1];
        }

        IEnumerable<TimeSpan> CalculateIntervals()
        {
            var random = new Random();
            var delta = -1;

            for (var i = 0; i < RetryLimit && delta < _maxInterval; i++)
            {
                delta = (int)Math.Min(_minInterval + Math.Pow(2, i) * random.Next(_lowInterval, _highInterval), _maxInterval);

                yield return TimeSpan.FromMilliseconds(delta);
            }
        }

        public override string ToString()
        {
            return $"Exponential (limit {RetryLimit}, min {_minInterval}ms, max {_maxInterval}ms";
        }
    }
}
