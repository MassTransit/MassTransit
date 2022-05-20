namespace BusHostTopologyMatch
{
    using System.Threading.Tasks;
    using MassTransit;

    public class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                cfg.Host("connection-string");
            });

            if (busControl.Topology is IServiceBusBusTopology serviceBusTopology)
            {

            }
        }
    }
}
