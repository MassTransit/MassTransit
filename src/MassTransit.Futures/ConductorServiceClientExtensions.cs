namespace MassTransit
{
    using Clients;
    using Clients.Contexts;
    using Conductor;
    using Conductor.Client;


    public static class ConductorServiceClientExtensions
    {
        /// <summary>
        /// Create a service client using the bus as the response endpoint for all requests and control traffic.
        /// </summary>
        /// <param name="bus">The bus instance</param>
        /// <param name="timeout">The default timeout for requests</param>
        /// <returns></returns>
        public static IServiceClient CreateServiceClient(this IBus bus, RequestTimeout timeout = default)
        {
            var clientFactoryContext = new BusClientFactoryContext(bus, timeout);
            var clientFactory = new ClientFactory(clientFactoryContext);
            var serviceClient = new ServiceClient(clientFactory, clientFactoryContext, bus);

            return serviceClient;
        }
    }
}
