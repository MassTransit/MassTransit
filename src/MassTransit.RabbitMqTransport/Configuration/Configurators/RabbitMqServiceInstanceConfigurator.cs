namespace MassTransit.RabbitMqTransport.Configurators
{
    using System;
    using Conductor.Configuration.Configurators;
    using Conductor.Server;


    public class RabbitMqServiceInstanceConfigurator :
        ServiceInstanceConfigurator<IRabbitMqHost, IRabbitMqReceiveEndpointConfigurator>
    {
        public RabbitMqServiceInstanceConfigurator(IReceiveConfigurator<IRabbitMqHost, IRabbitMqReceiveEndpointConfigurator> configurator, IRabbitMqHost host,
            IServiceInstance instance)
            : base(configurator, host, instance)
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
