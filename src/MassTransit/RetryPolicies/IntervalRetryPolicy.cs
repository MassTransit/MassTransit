namespace MassTransit.RetryPolicies
{
    using System;
    using System.Linq;


    public class IntervalRetryPolicy :
        IRetryPolicy
    {
        readonly IExceptionFilter _filter;

        public IntervalRetryPolicy(IExceptionFilter filter, params TimeSpan[] intervals)
        {
            if (intervals == null)
                throw new ArgumentNullException(nameof(intervals));
            if (intervals.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(intervals), "At least one interval must be specified");

            _filter = filter;

            Intervals = intervals;
        }

        public IntervalRetryPolicy(IExceptionFilter filter, params int[] intervals)
        {
            if (intervals == null)
                throw new ArgumentNullException(nameof(intervals));
            if (intervals.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(intervals), "At least one interval must be specified");

            _filter = filter;
            Intervals = intervals.Select(x => TimeSpan.FromMilliseconds(x)).ToArray();
        }

        public TimeSpan[] Intervals { get; }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Set(new
            {
                Policy = "Interval",
                Limit = Intervals.Length,
                Intervals
            });

            _filter.Probe(context);
        }

        RetryPolicyContext<T> IRetryPolicy.CreatePolicyContext<T>(T context)
        {
            return new IntervalRetryPolicyContext<T>(this, context);
        }

        public bool IsHandled(Exception exception)
        {
            return _filter.Match(exception);
        }

        public override string ToString()
        {
            return $"Interval (limit {Intervals.Length}, intervals {string.Join(";", Intervals.Take(5).Select(x => x.ToString()))})";
        }
    }
}
