namespace KafkaConsumer
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
                x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));

                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>();

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<KafkaMessage>("topic-name", "consumer-group-name", e =>
                        {
                            e.ConfigureConsumer<KafkaMessageConsumer>(context);
                        });
                    });
                });
            });
        }

        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            public Task Consume(ConsumeContext<KafkaMessage> context)
            {
                return Task.CompletedTask;
            }
        }

        public interface KafkaMessage
        {
            string Text { get; }
        }
    }
}
