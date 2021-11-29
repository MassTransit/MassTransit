namespace MassTransit.Configuration
{
    using Util;


    public class ConsumerConfigurationObservable :
        Connectable<IConsumerConfigurationObserver>,
        IConsumerConfigurationObserver
    {
        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            where TConsumer : class
        {
            ForEach(observer => observer.ConsumerConfigured(configurator));
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class
        {
            ForEach(observer => observer.ConsumerMessageConfigured(configurator));
        }
    }
}
