namespace UsageMessageSendCorrelation;

using System.Threading.Tasks;
using UsageContracts;
using MassTransit;

public class Program
{
    public static async Task Main()
    {
        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.SendTopology.UseCorrelationId<SubmitOrder>(x => x.OrderId);
        });
    }
}
