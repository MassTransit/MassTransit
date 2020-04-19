namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Transports.Metrics;
    using Util;


    public class Receiver :
        Supervisor,
        IReceiver
    {
        readonly ClientContext _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IBrokeredMessageReceiver _messageReceiver;

        public Receiver(ClientContext context, IBrokeredMessageReceiver messageReceiver)
        {
            _context = context;
            _messageReceiver = messageReceiver;

            messageReceiver.ZeroActivity += HandleDeliveryComplete;

            _deliveryComplete = TaskUtil.GetTask<bool>();
        }

        public DeliveryMetrics GetDeliveryMetrics()
        {
            return _messageReceiver.GetMetrics();
        }

        public virtual Task Start()
        {
            _context.OnMessageAsync(OnMessage, ExceptionHandler);

            SetReady();

            return TaskUtil.Completed;
        }

        protected Task ExceptionHandler(ExceptionReceivedEventArgs args)
        {
            if (!(args.Exception is OperationCanceledException))
                LogContext.Error?.Log(args.Exception, "Exception on Receiver {InputAddress} during {Action}", _context.InputAddress,
                    args.ExceptionReceivedContext.Action);

            if (_messageReceiver.ActiveDispatchCount == 0)
            {
                LogContext.Debug?.Log("Receiver shutdown completed: {InputAddress}", _context.InputAddress);

                _deliveryComplete.TrySetResult(true);

                SetCompleted(TaskUtil.Faulted<bool>(args.Exception));
            }

            return Task.CompletedTask;
        }

        async Task HandleDeliveryComplete()
        {
            if (IsStopping)
            {
                _deliveryComplete.TrySetResult(true);
            }
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            LogContext.Debug?.Log("Stopping receiver: {InputAddress}", _context.InputAddress);

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            await Completed.ConfigureAwait(false);

            await _context.CloseAsync(context.CancellationToken).ConfigureAwait(false);
        }

        async Task ActiveAndActualAgentsCompleted(StopSupervisorContext context)
        {
            await Task.WhenAll(context.Agents.Select(x => Completed)).OrCanceled(context.CancellationToken).ConfigureAwait(false);

            if (_messageReceiver.ActiveDispatchCount > 0)
                try
                {
                    await _deliveryComplete.Task.OrCanceled(context.CancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    LogContext.Warning?.Log("Stop canceled waiting for message consumers to complete: {InputAddress}", _context.InputAddress);
                }
        }

        Task OnMessage(IReceiverClient messageReceiver, Message message, CancellationToken cancellationToken)
        {
            if (IsStopping)
                return WaitForDeliveryComplete();

            return _messageReceiver.Handle(message, cancellationToken, context => AddReceiveContextPayloads(context, messageReceiver, message));
        }

        void AddReceiveContextPayloads(ReceiveContext receiveContext, IReceiverClient receiverClient, Message message)
        {
            MessageLockContext lockContext = new ReceiverClientMessageLockContext(receiverClient, message);

            receiveContext.GetOrAddPayload(() => lockContext);
            receiveContext.GetOrAddPayload(() => _context);
        }

        protected async Task WaitForDeliveryComplete()
        {
            try
            {
                await _deliveryComplete.Task.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Abandon message faulted during shutdown: {InputAddress}", _context.InputAddress);
            }
        }
    }
}
