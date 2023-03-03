namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;
    using Transports;


    public class EventHubReceiveLockContext :
        ReceiveLockContext
    {
        readonly ProcessEventArgs _eventArgs;
        readonly IProcessorLockContext _lockContext;

        public EventHubReceiveLockContext(ProcessEventArgs eventArgs, IProcessorLockContext lockContext)
        {
            _eventArgs = eventArgs;
            _lockContext = lockContext;
        }

        public Task Complete()
        {
            return _lockContext.Complete(_eventArgs);
        }

        public Task Faulted(Exception exception)
        {
            return _lockContext.Faulted(_eventArgs, exception);
        }

        public Task ValidateLockStatus()
        {
            return Task.CompletedTask;
        }
    }
}
