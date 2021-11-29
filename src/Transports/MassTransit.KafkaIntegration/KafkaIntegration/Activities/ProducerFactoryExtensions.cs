namespace MassTransit.KafkaIntegration.Activities
{
    using SagaStateMachine;


    static class ProducerFactoryExtensions
    {
        internal static ITopicProducer<T> GetProducer<T>(this PipeContext context)
            where T : class
        {
            var factory = context.GetStateMachineActivityFactory();

            ITopicProducer<T> producer = factory.GetService<ITopicProducer<T>>(context) ??
                throw new ProduceException($"TopicProducer<{TypeCache<T>.ShortName} not found");

            return producer;
        }
    }
}
