#nullable enable
namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class NoLockReceiveContext :
        ReceiveLockContext
    {
        public static readonly ReceiveLockContext Instance = new NoLockReceiveContext();

        NoLockReceiveContext()
        {
        }

        public Task Complete()
        {
            return Task.CompletedTask;
        }

        public Task Faulted(Exception exception)
        {
            return Task.CompletedTask;
        }

        public Task ValidateLockStatus()
        {
            return Task.CompletedTask;
        }
    }
}
