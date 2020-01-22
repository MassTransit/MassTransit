namespace MassTransit.RedisIntegration
{
    using Saga;


    public interface IVersionedSaga :
        ISaga
    {
        int Version { get; set; }
    }
}
