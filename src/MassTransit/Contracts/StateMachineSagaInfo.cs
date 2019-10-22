namespace MassTransit.Contracts
{
    public interface StateMachineSagaInfo
    {
        string Name { get; }

        string StateMachineType { get; }

        string InstanceType { get; }

        EventInfo[] Events { get; }

        StateInfo[] States { get; }
    }
}