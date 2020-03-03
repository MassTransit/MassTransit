namespace MassTransit.Contracts
{
    public interface ISagaNodeInfo :
        InstanceInfo
    {
        SagaInfo Saga { get; }
    }
}