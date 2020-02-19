namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Util;


    public struct ReceiverClientMessageLockContext :
        MessageLockContext
    {
        readonly IReceiverClient _receiverClient;
        readonly Message _message;

        public ReceiverClientMessageLockContext(IReceiverClient receiverClient, Message message)
        {
            _receiverClient = receiverClient;
            _message = message;
        }

        public Task Complete()
        {
            return _receiverClient.CompleteAsync(_message.SystemProperties.LockToken);
        }

        public Task Abandon(Exception exception)
        {
            return _receiverClient.AbandonAsync(_message.SystemProperties.LockToken, ExceptionUtil.GetExceptionHeaderDictionary(exception));
        }
    }
}
