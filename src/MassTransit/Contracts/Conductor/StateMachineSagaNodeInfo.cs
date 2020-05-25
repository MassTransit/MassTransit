namespace MassTransit.Contracts.Conductor
{
    using Metadata;


    public interface StateMachineSagaNodeInfo :
        InstanceInfo
    {
        StateMachineSagaInfo StateMachineSaga { get; }
    }
}
