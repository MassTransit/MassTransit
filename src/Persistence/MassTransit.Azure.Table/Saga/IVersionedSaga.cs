namespace MassTransit.Azure.Table.Saga
{
    using MassTransit.Saga;


    public interface IVersionedSaga :
        ISaga
    {
        string ETag { get; set; }
    }
}
