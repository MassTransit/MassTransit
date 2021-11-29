namespace MassTransit
{
    using System;


    public static class ServiceBusBusTopologyExtensions
    {
        public static IServiceBusBusTopology GetServiceBusBusTopology(this IBus bus)
        {
            if (bus.Topology is IServiceBusBusTopology hostTopology)
                return hostTopology;

            throw new ArgumentException("The bus is not an Azure Service Bus bus", nameof(bus));
        }
    }
}
