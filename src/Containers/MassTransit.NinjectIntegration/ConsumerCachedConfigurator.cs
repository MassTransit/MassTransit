namespace MassTransit.NinjectIntegration
{
    using Ninject;


    public class ConsumerCachedConfigurator<T> :
        ICachedConfigurator
        where T : class, IConsumer
    {
        public void Configure(IReceiveEndpointConfigurator configurator, IKernel kernel)
        {
            configurator.Consumer(new NinjectConsumerFactory<T>(kernel));
        }
    }
}
