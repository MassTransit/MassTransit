namespace AmazonSqsConsoleListener;

using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.UsingAmazonSqs((context, cfg) =>
                    {
                        cfg.Host("us-east-2", h =>
                        {
                            h.AccessKey("your-iam-access-key");
                            h.SecretKey("your-iam-secret-key");
                        });
                    });
                });
            })
            .Build()
            .RunAsync();
    }
}
