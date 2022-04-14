namespace MassTransit.Transactions
{
    using System.Threading.Tasks;


    public interface ITransactionalBus :
        IBus
    {
        Task Release();
    }
}
