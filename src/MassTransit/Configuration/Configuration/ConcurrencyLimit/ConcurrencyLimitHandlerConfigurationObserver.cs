namespace MassTransit.Configuration
{
    using Middleware;


    /// <summary>
    /// Configures a concurrency limit for a handler, on the handler configurator, which is constrained to
    /// the message type for that handler, and only applies to the handler.
    /// </summary>
    public class ConcurrencyLimitHandlerConfigurationObserver :
        IHandlerConfigurationObserver
    {
        public ConcurrencyLimitHandlerConfigurationObserver(int concurrentMessageLimit, string id = null)
        {
            Limiter = new ConcurrencyLimiter(concurrentMessageLimit, id);
        }

        public IConcurrencyLimiter Limiter { get; }

        public void HandlerConfigured<T>(IHandlerConfigurator<T> configurator)
            where T : class
        {
            var specification = new ConcurrencyLimitConsumePipeSpecification<T>(Limiter);

            configurator.AddPipeSpecification(specification);
        }
    }
}
