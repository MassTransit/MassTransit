namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;
    using GreenPipes.Util;
    using Transports;


    public class ProcessEventLockContext :
        ReceiveLockContext
    {
        readonly ProcessEventArgs _args;
        readonly CancellationToken _cancellationToken;
        readonly IProcessorLockContext _lockContext;

        public ProcessEventLockContext(IProcessorLockContext lockContext, ProcessEventArgs args, CancellationToken cancellationToken)
        {
            _lockContext = lockContext;
            _args = args;
            _cancellationToken = cancellationToken;
        }

        public Task Complete()
        {
            return _lockContext.Complete(_args, _cancellationToken);
        }

        public Task Faulted(Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task ValidateLockStatus()
        {
            return TaskUtil.Completed;
        }
    }
}
