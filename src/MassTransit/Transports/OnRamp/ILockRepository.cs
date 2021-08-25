using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public interface ILockRepository : IConnectionAndTransactionHolder
    {
        // Has the statements in which will row lock from the OutboxLocks table
        Task ObtainLock(string onRampName, string lockName);
    }
}
