namespace MassTransit.Conductor.Observers
{
    using Configuration.Configurators;
    using ConsumeConfigurators;
    using Courier.Contracts;
    using PipeConfigurators;


    public class ServiceEndpointConfigurationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly IServiceEndpointConfigurator _endpointConfigurator;

        public ServiceEndpointConfigurationObserver(IConsumePipeConfigurator configurator, IServiceEndpointConfigurator endpointConfigurator)
            : base(configurator)
        {
            _endpointConfigurator = endpointConfigurator;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            // Do not setup discovery for routing slip endpoints, these should be managed separately
            if (typeof(TMessage) == typeof(RoutingSlip))
                return;

            _endpointConfigurator.ConfigureMessage<TMessage>(configurator);
        }
    }
}
