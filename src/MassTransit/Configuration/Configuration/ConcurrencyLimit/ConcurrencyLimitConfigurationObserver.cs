namespace MassTransit.Configuration
{
    using Middleware;


    /// <summary>
    /// Adds a concurrency limit filter for each message type configured on the consume pipe
    /// </summary>
    public class ConcurrencyLimitConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        public ConcurrencyLimitConfigurationObserver(IConsumePipeConfigurator configurator, int concurrentMessageLimit, string id = null)
            : base(configurator)
        {
            Limiter = new ConcurrencyLimiter(concurrentMessageLimit, id);

            Connect(this);
        }

        public IConcurrencyLimiter Limiter { get; }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var specification = new ConcurrencyLimitConsumePipeSpecification<TMessage>(Limiter);

            configurator.AddPipeSpecification(specification);
        }
    }
}
