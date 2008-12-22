namespace MassTransit.Transports.Nms.Tests
{
    using System;
    using Configuration;
    using Serialization;
    using Util;

    public class NmsEndpointConfigurator :
        EndpointConfiguratorBase
    {
        public static IEndpoint New(Action<IEndpointConfigurator> action)
        {
            var configurator = new NmsEndpointConfigurator();

            action(configurator);

            return configurator.Create();
        }

        private IEndpoint Create()
        {
            Guard.Against.Null(Uri, "No Uri was specified for the endpoint");
            Guard.Against.Null(SerializerType, "No serializer type was specified for the endpoint");

            IMessageSerializer serializer = GetSerializer();

            IEndpoint endpoint = new NmsEndpoint(Uri, serializer);

            return endpoint;
        }
    }
}