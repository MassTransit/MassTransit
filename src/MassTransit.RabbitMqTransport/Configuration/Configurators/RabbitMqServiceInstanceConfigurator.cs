namespace MassTransit.RabbitMqTransport.Configurators
{
    using System;
    using Conductor.Configuration.Configurators;
    using Conductor.Server;


    public class RabbitMqServiceInstanceConfigurator :
        ServiceInstanceConfigurator<IRabbitMqReceiveEndpointConfigurator>
    {
        public RabbitMqServiceInstanceConfigurator(IReceiveConfigurator<IRabbitMqReceiveEndpointConfigurator> configurator, IServiceInstance instance)
            : base(configurator, instance)
        {
        }

        public override void ConfigureInstanceEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.AutoDelete = true;
            configurator.QueueExpiration = TimeSpan.FromMinutes(1);
        }

        protected override void ConfigureServiceEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.BindMessageExchanges = false;
        }
    }
}
