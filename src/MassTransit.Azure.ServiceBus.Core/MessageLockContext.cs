namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using System.Threading.Tasks;


    public interface MessageLockContext
    {
        Task Complete();

        Task Abandon(Exception exception);
    }
}
