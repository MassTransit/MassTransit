namespace MassTransit.KafkaIntegration.Subscriptions
{
    using GreenPipes;
    using Registration;


    public interface IKafkaTopic :
        ISpecification
    {
        string Name { get; }
        IKafkaConsumer CreateConsumer(IBusInstance busInstance);
    }
}
