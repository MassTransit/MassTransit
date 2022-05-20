namespace MassTransit
{
    using System.Threading.Tasks;
    using EventHubIntegration;
    using Microsoft.Extensions.DependencyInjection;
    using Testing;


    public static class EventHubTestHarnessExtensions
    {
        public static Task<IEventHubProducer> GetProducer(this ITestHarness harness, string eventHubName)
        {
            return harness.Scope.ServiceProvider.GetRequiredService<IEventHubProducerProvider>().GetProducer(eventHubName);
        }
    }
}
