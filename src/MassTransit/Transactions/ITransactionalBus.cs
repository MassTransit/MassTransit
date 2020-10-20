using System.Threading.Tasks;

namespace MassTransit.Transactions
{
    public interface ITransactionalBus :
        IBus
    {
        Task Release();
    }
}
