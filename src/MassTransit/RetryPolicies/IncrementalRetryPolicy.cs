namespace MassTransit.RetryPolicies
{
    using System;


    public class IncrementalRetryPolicy :
        IRetryPolicy
    {
        readonly IExceptionFilter _filter;

        public IncrementalRetryPolicy(IExceptionFilter filter, int retryLimit, TimeSpan initialInterval,
            TimeSpan intervalIncrement)
        {
            if (initialInterval < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(initialInterval),
                    "The initialInterval must be non-negative or -1, and it must be less than or equal to TimeSpan.MaxValue.");
            }

            if (intervalIncrement < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(intervalIncrement),
                    "The intervalIncrement must be non-negative or -1, and it must be less than or equal to TimeSpan.MaxValue.");
            }

            _filter = filter;
            RetryLimit = retryLimit;
            InitialInterval = initialInterval;
            IntervalIncrement = intervalIncrement;
        }

        public int RetryLimit { get; }

        public TimeSpan InitialInterval { get; }

        public TimeSpan IntervalIncrement { get; }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Set(new
            {
                Policy = "Incremental",
                Limit = RetryLimit,
                Initial = InitialInterval,
                Increment = IntervalIncrement
            });

            _filter.Probe(context);
        }

        RetryPolicyContext<T> IRetryPolicy.CreatePolicyContext<T>(T context)
        {
            return new IncrementalRetryPolicyContext<T>(this, context);
        }

        public bool IsHandled(Exception exception)
        {
            return _filter.Match(exception);
        }
    }
}
