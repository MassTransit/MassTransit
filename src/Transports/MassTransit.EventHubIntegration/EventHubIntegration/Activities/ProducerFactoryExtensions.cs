namespace MassTransit.EventHubIntegration.Activities
{
    using System.Threading.Tasks;
    using SagaStateMachine;


    static class ProducerFactoryExtensions
    {
        internal static Task<IEventHubProducer> GetProducer<T>(this BehaviorContext<T> context, ConsumeContext consumeContext, string eventHubName)
            where T : class, ISaga
        {
            var factory = context.GetStateMachineActivityFactory();

            var rider = factory.GetService<IEventHubRider>(context) ?? throw new ProduceException("EventHubRider not found");

            var producerProvider = rider.GetProducerProvider(consumeContext);

            return producerProvider.GetProducer(eventHubName);
        }
    }
}
