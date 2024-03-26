namespace MassTransit.EventHubIntegration.Activities
{
    using System.Threading.Tasks;


    static class ProducerFactoryExtensions
    {
        internal static Task<IEventHubProducer> GetProducer<T>(this BehaviorContext<T> context, ConsumeContext consumeContext, string eventHubName)
            where T : class, SagaStateMachineInstance
        {
            return context.GetServiceOrCreateInstance<IEventHubRider>()
                .GetProducerProvider(consumeContext)
                .GetProducer(eventHubName);
        }
    }
}
