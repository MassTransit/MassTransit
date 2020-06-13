namespace MassTransit.Azure.Cosmos
{
    using MassTransit.Saga;


    public interface IVersionedSaga : ISaga
    {
        string ETag { get; }
    }
}
