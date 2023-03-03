#nullable enable
namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class InMemoryReceiveLockContext :
        ReceiveLockContext
    {
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
