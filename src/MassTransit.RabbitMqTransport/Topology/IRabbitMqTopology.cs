namespace MassTransit.RabbitMqTransport.Topology
{
    public interface IRabbitMqTopology
    {
        IRabbitMqConsumeTopology ConsumeTopology { get; }
        IRabbitMqSendTopology SendTopology { get; }
        IRabbitMqPublishTopology PublishTopology { get; }
    }
}