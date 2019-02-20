namespace MassTransit.PipeConfigurators
{
    using ConsumeConfigurators;
    using Pipeline.Filters.ConcurrencyLimit;


    /// <summary>
    /// Configures a concurrency limit for a handler, on the handler configurator, which is constrained to
    /// the message type for that handler, and only applies to the handler.
    /// </summary>
    public class ConcurrencyLimitHandlerConfigurationObserver :
        IHandlerConfigurationObserver
    {
        readonly IConcurrencyLimiter _limiter;

        public ConcurrencyLimitHandlerConfigurationObserver(int concurrentMessageLimit, string id = null)
        {
            _limiter = new ConcurrencyLimiter(concurrentMessageLimit, id);
        }

        public IConcurrencyLimiter Limiter => _limiter;

        public void HandlerConfigured<T>(IHandlerConfigurator<T> configurator)
            where T : class
        {
            var specification = new ConcurrencyLimitConsumePipeSpecification<T>(_limiter);

            configurator.AddPipeSpecification(specification);
        }
    }
}
