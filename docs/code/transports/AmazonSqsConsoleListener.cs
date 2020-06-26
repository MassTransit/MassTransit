namespace AmazonSqsConsoleListener
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();
            services.AddMassTransit(x =>
            {
                x.UsingAmazonSqs((context, cfg) =>
                {
                    cfg.Host("us-east-2", h =>
                    {
                        h.AccessKey("your-iam-access-key");
                        h.SecretKey("your-iam-secret-key");

                        // following are OPTIONAL

                        // specify a scope for all queues
                        h.Scope("dev");

                        // scope topics as well
                        h.EnableScopedTopics();
                    });
                });
            });
        }
    }
}
