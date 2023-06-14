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
            return _message.AcknowledgeAsync();
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
