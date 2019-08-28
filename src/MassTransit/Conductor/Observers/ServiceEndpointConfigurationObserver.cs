namespace MassTransit.Conductor.Observers
{
    using ConsumeConfigurators;
    using GreenPipes;
    using GreenPipes.Specifications;
    using PipeConfigurators;
    using Pipeline;
    using Server;


    public class ServiceEndpointConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly IServiceEndpoint _endpoint;

        public ServiceEndpointConfigurationObserver(IConsumePipeConfigurator configurator, IServiceEndpoint endpoint)
            : base(configurator)
        {
            _endpoint = endpoint;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            IFilter<ConsumeContext<TMessage>> filter = new ConductorMessageFilter<TMessage>(_endpoint.GetMessageEndpoint<TMessage>());

            configurator.AddPipeSpecification(new FilterPipeSpecification<ConsumeContext<TMessage>>(filter));
        }
    }
}
