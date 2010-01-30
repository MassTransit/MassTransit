namespace MassTransit.Transports.RabbitMq
{
    using System;
    using Magnum;
    using MassTransit.Configuration;

    public class RabbitMqEndpointConfigurator :
        EndpointConfiguratorBase
    {
        public static IEndpoint New(Action<IEndpointConfigurator> action)
        {
            var configurator = new RabbitMqEndpointConfigurator();

            action(configurator);

            return configurator.Create();
        }

        private IEndpoint Create()
        {
            Guard.Against.Null(Uri, "No Uri was specified for the endpoint");
            Guard.Against.Null(SerializerType, "No serializer type was specified for the endpoint");

            IEndpoint endpoint = RabbitMqEndpointFactory.New(new CreateEndpointSettings(Uri)
            {
                Serializer = GetSerializer(),
            });

            return endpoint;
        }
    }
}