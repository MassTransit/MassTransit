namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    /// <summary>
    /// Adds a concurrency limit filter to the message pipe.
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class ConcurrencyLimitConsumePipeSpecification<T> :
        IPipeSpecification<ConsumeContext<T>>
        where T : class
    {
        readonly IConcurrencyLimiter _limiter;

        public ConcurrencyLimitConsumePipeSpecification(IConcurrencyLimiter limiter)
        {
            _limiter = limiter;
        }

        public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
        {
            var filter = new ConsumeConcurrencyLimitFilter<T>(_limiter);

            builder.AddFilter(filter);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_limiter.Limit < 1)
                yield return this.Failure("ConcurrencyLimit", "must be >= 1");
        }
    }
}
