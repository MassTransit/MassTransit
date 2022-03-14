namespace KafkaTopicTopology
{
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
                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<KafkaMessage>("topic-name", "consumer-group-name", e =>
                        {
                            e.CreateIfMissing(t =>
                            {
                                t.NumPartitions = 2; //number of partitions
                                t.ReplicationFactor = 1; //number of replicas
                            });
                        });
                    });
                });
            });
        }


        public interface KafkaMessage
        {
        }
    }
}
