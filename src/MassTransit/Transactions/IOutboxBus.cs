using System.Threading.Tasks;

namespace MassTransit.Transactions
{
    public interface IOutboxBus :
        IBus
    {
        Task Release();
    }
}
