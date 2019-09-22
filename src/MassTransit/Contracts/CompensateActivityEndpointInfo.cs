namespace MassTransit.Contracts
{
    public interface CompensateActivityEndpointInfo :
        EndpointInfo
    {
        ActivityInfo Activity { get; }
    }
}