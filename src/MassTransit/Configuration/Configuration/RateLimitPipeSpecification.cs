namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Middleware;


    public class RateLimitPipeSpecification<T> :
        IPipeSpecification<T>
        where T : class, PipeContext
    {
        readonly TimeSpan _interval;
        readonly int _rateLimit;
        readonly IPipeRouter _router;

        public RateLimitPipeSpecification(int rateLimit, TimeSpan interval, IPipeRouter router = null)
        {
            _rateLimit = rateLimit;
            _interval = interval;
            _router = router;
        }

        public void Apply(IPipeBuilder<T> builder)
        {
            var filter = new RateLimitFilter<T>(_rateLimit, _interval);

            builder.AddFilter(filter);

            _router?.ConnectPipe(filter);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_rateLimit < 1)
                yield return this.Failure("RateLimit", "must be >= 1");
            if (_interval <= TimeSpan.Zero)
                yield return this.Failure("Interval", "must be > 0");
        }
    }
}
