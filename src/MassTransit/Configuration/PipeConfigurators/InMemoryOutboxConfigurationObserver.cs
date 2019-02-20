namespace MassTransit.PipeConfigurators
{
    using ConsumeConfigurators;


    public class InMemoryOutboxConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        public InMemoryOutboxConfigurationObserver(IConsumePipeConfigurator configurator)
            : base(configurator)
        {
            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var specification = new InMemoryOutboxSpecification<TMessage>();

            configurator.AddPipeSpecification(specification);
        }
    }
}
