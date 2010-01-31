namespace MassTransit.Transports.RabbitMq
{
    using System;
    using Exceptions;
    using Magnum;
    using RabbitMQ.Client;
    using Serialization;

    public static class RabbitMqEndpointFactory
    {
        private static ConnectionFactory _factory = new ConnectionFactory();
        static IProtocol _protocol = Protocols.AMQP_0_8;
       
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
                EnsureProtocolIsCorrect(settings.Address.Uri);
                EnsureProtocolIsCorrect(settings.ErrorAddress.Uri);

                var transport = new RabbitMqTransport(settings.Address, GetConnection(settings.Address.Uri));

                var errorSettings = new CreateEndpointSettings(settings.ErrorAddress, settings);
                var errorTransport = new RabbitMqTransport(errorSettings.Address, GetConnection(errorSettings.Address.Uri));

                var endpoint = new RabbitMqEndpoint(settings.Address, settings.Serializer, transport, errorTransport);

                return endpoint;
            }
            catch (Exception ex)
            {
                throw new EndpointException(settings.Address.Uri, "Failed to create NMS endpoint", ex);
            }
        }

        static void EnsureProtocolIsCorrect(Uri address)
        {
            if(address.Scheme != "rabbitmq") 
                throw new EndpointException(address, "Address must start with 'rabbitmq' not '{0}'".FormatWith(address.Scheme));
        }

        private static IConnection GetConnection(Uri address)
        {
            var rabbitMqAddress = new UriBuilder("amqp-{0}-{1}".FormatWith(_protocol.MajorVersion, _protocol.MinorVersion), address.Host, _protocol.DefaultPort).Uri;
            return _factory.CreateConnection(rabbitMqAddress);
        }
    }
}