namespace MassTransit.Contracts.Conductor
{
    using Metadata;


    public interface ISagaNodeInfo :
        InstanceInfo
    {
        SagaInfo Saga { get; }
    }
}
