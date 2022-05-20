namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Internals;
    using Logging;
    using MassTransit.Middleware;
    using Transports;
    using Util;


    public class Receiver :
        Agent,
        IReceiver
    {
        readonly ClientContext _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IServiceBusMessageReceiver _messageReceiver;

        public Receiver(ClientContext context, IServiceBusMessageReceiver messageReceiver)
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

            return _context.StartAsync();
        }

        protected async Task ExceptionHandler(ProcessErrorEventArgs args)
        {
            var requiresRecycle = args.Exception switch
            {
                MessageTimeToLiveExpiredException _ => false,
                MessageLockExpiredException _ => false,
                ServiceBusException { Reason: ServiceBusFailureReason.MessageLockLost } => false,
                ServiceBusException { Reason: ServiceBusFailureReason.ServiceCommunicationProblem } => true,
                ServiceBusException { IsTransient: true } => false,
                _ => true
            };

            if (args.Exception is ServiceBusException { IsTransient: true, Reason: ServiceBusFailureReason.ServiceCommunicationProblem })
            {
                LogContext.Debug?.Log(args.Exception,
                    "ServiceBusException on Receiver {InputAddress} during {Action} ActiveDispatchCount({activeDispatch}) ErrorRequiresRecycle({requiresRecycle})",
                    _context.InputAddress, args.ErrorSource, _messageReceiver.ActiveDispatchCount, requiresRecycle);
            }
            else if (args.Exception is WebSocketException exception)
            {
                LogContext.Debug?.Log(exception,
                    "WebSocketException on Receiver {InputAddress} code {Code} ActiveDispatchCount({activeDispatch}) ErrorRequiresRecycle({requiresRecycle})",
                    _context.InputAddress, exception.WebSocketErrorCode, _messageReceiver.ActiveDispatchCount, requiresRecycle);
            }
            else if (args.Exception is ObjectDisposedException { ObjectName: "$cbs" })
            {
                // don't log this one
            }
            else if (args.Exception is ServiceBusException { Reason: ServiceBusFailureReason.MessageLockLost })
            {
                // don't log this one
            }
            else if (!(args.Exception is OperationCanceledException) && !(args.Exception.InnerException is TimeoutException))
            {
                EnabledLogger? logger = requiresRecycle ? LogContext.Error : LogContext.Warning;

                logger?.Log(args.Exception,
                    "Exception on Receiver {InputAddress} during {Action} ActiveDispatchCount({activeDispatch}) ErrorRequiresRecycle({requiresRecycle})",
                    _context.InputAddress, args.ErrorSource, _messageReceiver.ActiveDispatchCount, requiresRecycle);
            }

            if (requiresRecycle)
            {
                await _context.NotifyFaulted(args.Exception, args.EntityPath).ConfigureAwait(false);

                _deliveryComplete.TrySetResult(false);

            #pragma warning disable 4014
                Task.Run(() => this.Stop($"Receiver Exception: {args.Exception.Message}"));
            #pragma warning restore 4014
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

        Task OnMessage(ProcessMessageEventArgs messageReceiver, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            return _messageReceiver.Handle(message, cancellationToken, context => AddReceiveContextPayloads(context, messageReceiver, message));
        }

        void AddReceiveContextPayloads(ReceiveContext receiveContext, ProcessMessageEventArgs receiverClient, ServiceBusReceivedMessage message)
        {
            MessageLockContext lockContext = new ServiceBusMessageLockContext(receiverClient, message);

            receiveContext.GetOrAddPayload(() => lockContext);
            receiveContext.GetOrAddPayload(() => _context);
        }
    }
}
