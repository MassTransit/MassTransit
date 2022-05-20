namespace KafkaProducer
{
    using System;
    using System.Threading;
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
                x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));

                x.AddRider(rider =>
                {
                    rider.AddProducer<KafkaMessage>("topic-name");

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
            try
            {
                var producer = provider.GetRequiredService<ITopicProducer<KafkaMessage>>();
                do
                {
                    string value = await Task.Run(() =>
                    {
                        Console.WriteLine("Enter text (or quit to exit)");
                        Console.Write("> ");
                        return Console.ReadLine();
                    });

                    if("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                        break;

                    await producer.Produce(new
                    {
                        Text = value
                    });
                }
                while (true);
            }
            finally
            {
                await busControl.StopAsync();
            }
        }

        public interface KafkaMessage
        {
            string Text { get; }
        }
    }
}
