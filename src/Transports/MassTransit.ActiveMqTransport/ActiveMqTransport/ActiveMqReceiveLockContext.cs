namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Transports;


    public class ActiveMqReceiveLockContext :
        ReceiveLockContext
    {
        readonly IMessage _message;

        public ActiveMqReceiveLockContext(IMessage message)
        {
            _message = message;
        }

        public Task Complete()
        {
            _message.Acknowledge();

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
