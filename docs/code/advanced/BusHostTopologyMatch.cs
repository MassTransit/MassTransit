namespace BusHostTopologyMatch
{
    using System;
    using System.Threading.Tasks;
    using BatchingConsumer;
    using MassTransit;
    using MassTransit.Azure.ServiceBus.Core;
    using MassTransit.Azure.ServiceBus.Core.Topology;

    public class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                cfg.Host("connection-string");
            });

            if (busControl.Topology is IServiceBusHostTopology serviceBusTopology)
            {

            }
        }
    }
}