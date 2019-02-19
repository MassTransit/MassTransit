namespace MassTransit.PipeConfigurators
{
    using ConsumeConfigurators;
    using Pipeline.Filters.ConcurrencyLimit;


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
        readonly IConcurrencyLimiter _limiter;

        public ConcurrencyLimitConsumerConfigurationObserver(IConsumerConfigurator<TConsumer> configurator, int concurrentMessageLimit, string id = null)
        {
            _configurator = configurator;
            _limiter = new ConcurrencyLimiter(concurrentMessageLimit, id);
        }

        public IConcurrencyLimiter Limiter => _limiter;

        void IConsumerConfigurationObserver.ConsumerConfigured<T>(IConsumerConfigurator<T> configurator)
        {
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<T, TMessage>(IConsumerMessageConfigurator<T, TMessage> configurator)
        {
            var specification = new ConcurrencyLimitConsumePipeSpecification<TMessage>(_limiter);

            _configurator.Message<TMessage>(x => x.AddPipeSpecification(specification));
        }
    }
}
