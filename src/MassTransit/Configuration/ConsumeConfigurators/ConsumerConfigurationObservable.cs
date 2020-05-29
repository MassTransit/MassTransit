namespace MassTransit.ConsumeConfigurators
{
    using GreenPipes.Util;


    public class ConsumerConfigurationObservable :
        Connectable<IConsumerConfigurationObserver>,
        IConsumerConfigurationObserver
    {
        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            where TConsumer : class
        {
            All(observer =>
            {
                observer.ConsumerConfigured(configurator);

                return true;
            });
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class
        {
            All(observer =>
            {
                observer.ConsumerMessageConfigured(configurator);

                return true;
            });
        }
    }
}
