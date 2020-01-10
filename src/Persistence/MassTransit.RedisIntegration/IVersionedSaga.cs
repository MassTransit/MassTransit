namespace MassTransit.RedisIntegration
{
    using MassTransit.Saga;


    public interface IVersionedSaga :
        ISaga
    {
        int Version { get; set; }
    }
}
