namespace MassTransit.Contracts
{
    public interface ExecuteActivityEndpointInfo :
        EndpointInfo
    {
        ActivityInfo Activity { get; }
    }
}