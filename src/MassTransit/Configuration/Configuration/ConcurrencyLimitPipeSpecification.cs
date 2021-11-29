namespace MassTransit.Configuration
{
    using System.Collections.Generic;
    using Middleware;


    /// <summary>
    /// Configures a concurrency limit on the pipe. If the management endpoint is specified,
    /// the consumer and appropriate mediator is created to handle the adjustment of the limit.
    /// </summary>
    /// <typeparam name="T">The message type being limited</typeparam>
    public class ConcurrencyLimitPipeSpecification<T> :
        IPipeSpecification<T>
        where T : class, PipeContext
    {
        readonly int _concurrencyLimit;

        readonly IPipeRouter _router;

        public ConcurrencyLimitPipeSpecification(int concurrencyLimit, IPipeRouter router = null)
        {
            _concurrencyLimit = concurrencyLimit;

            _router = router;
        }

        public void Apply(IPipeBuilder<T> builder)
        {
            var filter = new ConcurrencyLimitFilter<T>(_concurrencyLimit);

            builder.AddFilter(filter);

            _router?.ConnectPipe(filter);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_concurrencyLimit < 1)
                yield return this.Failure("ConcurrencyLimit", "must be >= 1");
        }
    }
}
