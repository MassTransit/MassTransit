namespace MassTransit
{
    using Microsoft.Extensions.DependencyInjection;
    using Testing;


    public static class KafkaTestHarnessExtensions
    {
        public static ITopicProducer<T> GetProducer<T>(this ITestHarness harness)
            where T : class
        {
            return harness.Scope.ServiceProvider.GetRequiredService<ITopicProducer<T>>();
        }
    }
}
