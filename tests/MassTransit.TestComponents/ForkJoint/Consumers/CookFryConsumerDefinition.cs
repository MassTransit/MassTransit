namespace MassTransit.TestComponents.ForkJoint.Consumers
{
    using Conductor;
    using Conductor.Directory;
    using Contracts;
    using Definition;


    public class CookFryConsumerDefinition :
        ConsumerDefinition<CookFryConsumer>,
        IConfigureServiceDirectory
    {
        public CookFryConsumerDefinition()
        {
            ConcurrentMessageLimit = 32;
        }

        public void Configure(IServiceDirectoryConfigurator directoryConfigurator)
        {
            directoryConfigurator.AddService<CookFry, FryReady>(x => x.Consumer<CookFryConsumer>());
        }
    }
}
