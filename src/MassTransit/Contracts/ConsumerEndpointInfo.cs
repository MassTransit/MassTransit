namespace MassTransit.Contracts
{
    public interface ConsumerEndpointInfo :
        EndpointInfo
    {
        ConsumerInfo Consumer { get; }
    }
}