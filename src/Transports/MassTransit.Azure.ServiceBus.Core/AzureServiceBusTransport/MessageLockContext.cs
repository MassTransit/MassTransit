namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading.Tasks;


    public interface MessageLockContext
    {
        Task Complete();

        Task Abandon(Exception exception);
        Task DeadLetter();
        Task DeadLetter(Exception exception);
    }
}
