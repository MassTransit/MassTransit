namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
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
        protected readonly IDeliveryTracker Tracker;

        public Receiver(ClientContext context, IBrokeredMessageReceiver messageReceiver)
        {
            _context = context;
            _messageReceiver = messageReceiver;

            Tracker = new DeliveryTracker(HandleDeliveryComplete);
            _deliveryComplete = TaskUtil.GetTask<bool>();
        }

        public DeliveryMetrics GetDeliveryMetrics()
        {
            return Tracker.GetDeliveryMetrics();
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

            if (Tracker.ActiveDeliveryCount == 0)
            {
                LogContext.Debug?.Log("Receiver shutdown completed: {InputAddress}", _context.InputAddress);

                _deliveryComplete.TrySetResult(true);

                SetCompleted(TaskUtil.Faulted<bool>(args.Exception));
            }

            return Task.CompletedTask;
        }

        void HandleDeliveryComplete()
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

            if (Tracker.ActiveDeliveryCount > 0)
                try
                {
                    await _deliveryComplete.Task.OrCanceled(context.CancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    LogContext.Warning?.Log("Stop canceled waiting for message consumers to complete: {InputAddress}", _context.InputAddress);
                }
        }

        async Task OnMessage(IReceiverClient messageReceiver, Message message, CancellationToken cancellationToken)
        {
            if (IsStopping)
            {
                await WaitAndAbandonMessage(messageReceiver, message).ConfigureAwait(false);
                return;
            }

            using (var delivery = Tracker.BeginDelivery())
            {
                await _messageReceiver.Handle(message, context => AddReceiveContextPayloads(context, messageReceiver, message)).ConfigureAwait(false);
            }
        }

        void AddReceiveContextPayloads(ReceiveContext receiveContext, IReceiverClient receiverClient, Message message)
        {
            MessageLockContext lockContext = new ReceiverClientMessageLockContext(receiverClient, message);

            receiveContext.GetOrAddPayload(() => lockContext);
            receiveContext.GetOrAddPayload(() => _context.GetPayload<NamespaceContext>());
        }

        protected async Task WaitAndAbandonMessage(IReceiverClient receiverClient, Message message)
        {
            try
            {
                await _deliveryComplete.Task.ConfigureAwait(false);

                await receiverClient.AbandonAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Abandon message faulted during shutdown: {InputAddress}", _context.InputAddress);
            }
        }
    }
}
