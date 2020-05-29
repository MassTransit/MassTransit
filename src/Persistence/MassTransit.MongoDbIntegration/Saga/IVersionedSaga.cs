namespace MassTransit.MongoDbIntegration.Saga
{
    using MassTransit.Saga;


    public interface IVersionedSaga :
        ISaga
    {
        int Version { get; set; }
    }
}
