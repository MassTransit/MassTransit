namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using Topology;


    public static class ServiceBusHostTopologyExtensions
    {
        public static IServiceBusHostTopology GetServiceBusHostTopology(this IBus bus)
        {
            if (bus.Topology is IServiceBusHostTopology hostTopology)
                return hostTopology;

            throw new ArgumentException("The bus is not an Azure Service Bus bus", nameof(bus));
        }
    }
}
