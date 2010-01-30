namespace MassTransit.Transports.RabbitMq
{
    using System;
    using Exceptions;
    using Magnum;
    using Serialization;

    public static class RabbitMqEndpointFactory
    {
        public static IEndpoint New(IEndpointAddress address, IMessageSerializer serializer)
        {
            return New(new CreateEndpointSettings(address)
            {
                Serializer = serializer,
            });
        }

        public static IEndpoint New(CreateEndpointSettings settings)
        {
            try
            {
                Guard.Against.Null(settings.Address, "An address for the endpoint must be specified");
                Guard.Against.Null(settings.ErrorAddress, "An error address for the endpoint must be specified");
                Guard.Against.Null(settings.Serializer, "A message serializer for the endpoint must be specified");

                var transport = new RabbitMqTransport(settings.Address);

                var errorSettings = new CreateEndpointSettings(settings.ErrorAddress, settings);
                ITransport errorTransport = new RabbitMqTransport(errorSettings.Address);

                var endpoint = new RabbitMqEndpoint(settings.Address, settings.Serializer, transport, errorTransport);

                return endpoint;
            }
            catch (Exception ex)
            {
                throw new EndpointException(settings.Address.Uri, "Failed to create NMS endpoint", ex);
            }
        }
    }
}