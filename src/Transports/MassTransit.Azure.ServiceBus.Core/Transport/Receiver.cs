namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Logging;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Transports.Metrics;
    using Util;


    public class Receiver :
        Agent,
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

        protected async Task ExceptionHandler(ExceptionReceivedEventArgs args)
        {
            var requiresRecycle = args.Exception switch
            {
                MessageLockLostException _ => false,
                MessageTimeToLiveExpiredException _ => false,
                ServiceBusCommunicationException _ => true,
                ServiceBusException { IsTransient: true } => false,
                _ => true
            };

            if (args.Exception is ServiceBusCommunicationException { IsTransient: true })
            {
                LogContext.Debug?.Log(args.Exception,
                    "Exception on Receiver {InputAddress} during {Action} ActiveDispatchCount({activeDispatch}) ErrorRequiresRecycle({requiresRecycle})",
                    _context.InputAddress, args.ExceptionReceivedContext.Action, _messageReceiver.ActiveDispatchCount, requiresRecycle);
            }
            else if (args.Exception is ObjectDisposedException { ObjectName: "$cbs" })
            {
                // don't log this one
            }
            else if (!(args.Exception is OperationCanceledException))
            {
                EnabledLogger? logger = requiresRecycle ? LogContext.Error : LogContext.Warning;

                logger?.Log(args.Exception,
                    "Exception on Receiver {InputAddress} during {Action} ActiveDispatchCount({activeDispatch}) ErrorRequiresRecycle({requiresRecycle})",
                    _context.InputAddress, args.ExceptionReceivedContext.Action, _messageReceiver.ActiveDispatchCount, requiresRecycle);
            }

            if (requiresRecycle)
            {
                if (_deliveryComplete.TrySetResult(false))
                {
                    await _context.NotifyFaulted(args.Exception, args.ExceptionReceivedContext.EntityPath).ConfigureAwait(false);

                    await this.Stop($"Receiver Exception: {args.Exception.Message}").ConfigureAwait(false);
                }
            }
        }

        async Task HandleDeliveryComplete()
        {
            if (IsStopping)
                _deliveryComplete.TrySetResult(true);
        }

        protected override async Task StopAgent(StopContext context)
        {
            await _context.ShutdownAsync().ConfigureAwait(false);

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            await Completed.ConfigureAwait(false);

            await _context.CloseAsync().ConfigureAwait(false);

            LogContext.Debug?.Log("Receiver stopped: {InputAddress}", _context.InputAddress);
        }

        async Task ActiveAndActualAgentsCompleted(StopContext context)
        {
            if (_messageReceiver.ActiveDispatchCount > 0)
            {
                try
                {
                    await _deliveryComplete.Task.OrCanceled(context.CancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    LogContext.Warning?.Log("Stop canceled waiting for message consumers to complete: {InputAddress}", _context.InputAddress);
                }
            }
        }

        Task OnMessage(IReceiverClient messageReceiver, Message message, CancellationToken cancellationToken)
        {
            return _messageReceiver.Handle(message, cancellationToken, context => AddReceiveContextPayloads(context, messageReceiver, message));
        }

        void AddReceiveContextPayloads(ReceiveContext receiveContext, IReceiverClient receiverClient, Message message)
        {
            MessageLockContext lockContext = new ReceiverClientMessageLockContext(receiverClient, message);

            receiveContext.GetOrAddPayload(() => lockContext);
            receiveContext.GetOrAddPayload(() => _context);
        }
    }
}
