namespace MassTransit.RabbitMqTransport.Topology
{
    using GreenPipes;


    public interface IRabbitMqConsumeTopologySpecification :
        ISpecification
    {
        void Apply(IRabbitMqConsumeTopologyBuilder builder);
    }
}