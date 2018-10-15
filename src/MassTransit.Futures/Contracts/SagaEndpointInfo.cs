namespace MassTransit.Contracts
{
    public interface SagaEndpointInfo :
        EndpointInfo
    {
        SagaInfo Saga { get; }
    }
}