namespace MassTransit.Configuration
{
    using Middleware;


    /// <summary>
    /// Configures a concurrency limit for a consumer, on the consumer configurator, which is constrained to
    /// the message types for that consumer, and only applies to the consumer prior to the consumer factory.
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    public class ConcurrencyLimitConsumerConfigurationObserver<TConsumer> :
        IConsumerConfigurationObserver
        where TConsumer : class
    {
        readonly IConsumerConfigurator<TConsumer> _configurator;

        public ConcurrencyLimitConsumerConfigurationObserver(IConsumerConfigurator<TConsumer> configurator, int concurrentMessageLimit, string id = null)
        {
            _configurator = configurator;
            Limiter = new ConcurrencyLimiter(concurrentMessageLimit, id);
        }

        public IConcurrencyLimiter Limiter { get; }

        void IConsumerConfigurationObserver.ConsumerConfigured<T>(IConsumerConfigurator<T> configurator)
        {
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<T, TMessage>(IConsumerMessageConfigurator<T, TMessage> configurator)
        {
            var specification = new ConcurrencyLimitConsumePipeSpecification<TMessage>(Limiter);

            _configurator.Message<TMessage>(x => x.AddPipeSpecification(specification));
        }
    }
}
