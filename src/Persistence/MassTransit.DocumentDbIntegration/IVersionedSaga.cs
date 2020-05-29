namespace MassTransit.DocumentDbIntegration
{
    using MassTransit.Saga;


    public interface IVersionedSaga : ISaga
    {
        string ETag { get; }
    }
}
