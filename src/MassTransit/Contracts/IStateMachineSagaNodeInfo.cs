namespace MassTransit.Contracts
{
    public interface IStateMachineSagaNodeInfo :
        InstanceInfo
    {
        StateMachineSagaInfo StateMachineSaga { get; }
    }
}