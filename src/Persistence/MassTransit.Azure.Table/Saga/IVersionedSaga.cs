namespace MassTransit.Azure.Table
{
    using Saga;


    public interface IVersionedSaga :
        ISaga
    {
        string ETag { get; set; }
    }
}
