namespace MassTransit
{
    using Conductor.Client;


    public static class ConductorServiceClientExtensions
    {
        /// <summary>
        /// Create a service client using the bus as the response endpoint for all requests and control traffic.
        /// </summary>
        /// <param name="bus">The bus instance</param>
        /// <param name="timeout">The default timeout for requests</param>
        /// <returns></returns>
        public static IClientFactory CreateServiceClient(this IBus bus, RequestTimeout timeout = default)
        {
            var clientFactory = bus.CreateClientFactory(timeout);

            var serviceClient = new ServiceClient(clientFactory, bus);
            var serviceClientFactory = new ServiceClientFactory(serviceClient, clientFactory);

            return serviceClientFactory;
        }
    }
}
