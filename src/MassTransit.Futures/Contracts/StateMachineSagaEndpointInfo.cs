namespace MassTransit.Contracts
{
    public interface StateMachineSagaEndpointInfo :
        EndpointInfo
    {
        StateMachineSagaInfo StateMachineSaga { get; }
    }
}