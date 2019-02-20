namespace MassTransit.PipeConfigurators
{
    using ConsumeConfigurators;


    public class InMemoryOutboxConsumerConfigurationObserver<TConsumer> :
        IConsumerConfigurationObserver
        where TConsumer : class
    {
        readonly IConsumerConfigurator<TConsumer> _configurator;

        public InMemoryOutboxConsumerConfigurationObserver(IConsumerConfigurator<TConsumer> configurator)
        {
            _configurator = configurator;
        }

        void IConsumerConfigurationObserver.ConsumerConfigured<T>(IConsumerConfigurator<T> configurator)
        {
        }

        void IConsumerConfigurationObserver.ConsumerMessageConfigured<T, TMessage>(IConsumerMessageConfigurator<T, TMessage> configurator)
        {
            var specification = new InMemoryOutboxSpecification<TMessage>();

            _configurator.Message<TMessage>(x => x.AddPipeSpecification(specification));
        }
    }
}