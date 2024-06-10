namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Logging;
    using Transports;


    public class Receiver :
        ConsumerAgent<long>,
        IReceiver
    {
        readonly ClientContext _clientContext;
        readonly ServiceBusReceiveEndpointContext _context;

        public Receiver(ClientContext clientClientContext, ServiceBusReceiveEndpointContext context)
            : base(context)
        {
            _clientContext = clientClientContext;
            _context = context;

            TrySetManualConsumeTask();
        }

        public virtual void Start()
        {
            _clientContext.OnMessageAsync(OnMessage, ExceptionHandler);

            SetReady(_clientContext.StartAsync());
        }

        protected async Task ExceptionHandler(ProcessErrorEventArgs args)
        {
            var requiresRecycle = args.Exception switch
            {
                MessageTimeToLiveExpiredException _ => false,
                MessageLockExpiredException _ => false,

                ServiceBusException { Reason: ServiceBusFailureReason.MessageLockLost } => false,
                ServiceBusException { Reason: ServiceBusFailureReason.SessionLockLost } => false,

                ServiceBusException { Reason: ServiceBusFailureReason.ServiceCommunicationProblem } => true,
                ServiceBusException { Reason: ServiceBusFailureReason.MessagingEntityNotFound } => true,
                ServiceBusException { Reason: ServiceBusFailureReason.MessagingEntityDisabled } => true,

                ServiceBusException { IsTransient: true } => false,

                _ => true
            };

            switch (args.Exception)
            {
                case ServiceBusException { IsTransient: true, Reason: ServiceBusFailureReason.ServiceCommunicationProblem }:
                    LogContext.Debug?.Log(args.Exception,
                        "ServiceBusException on Receiver {InputAddress} during {Action} ActiveDispatchCount({activeDispatch}) ErrorRequiresRecycle({requiresRecycle})",
                        _clientContext.InputAddress, args.ErrorSource, ActiveDispatchCount, requiresRecycle);
                    break;
                case WebSocketException exception:
                    LogContext.Debug?.Log(exception,
                        "WebSocketException on Receiver {InputAddress} code {Code} ActiveDispatchCount({activeDispatch}) ErrorRequiresRecycle({requiresRecycle})",
                        _clientContext.InputAddress, exception.WebSocketErrorCode, ActiveDispatchCount, requiresRecycle);
                    break;
                case ObjectDisposedException { ObjectName: "$cbs" }:
                case ServiceBusException { Reason: ServiceBusFailureReason.MessageLockLost }:
                case ServiceBusException { Reason: ServiceBusFailureReason.SessionLockLost }:
                    // don't log those
                    break;
                default:
                {
                    if (!(args.Exception is OperationCanceledException) && !(args.Exception.InnerException is TimeoutException))
                    {
                        EnabledLogger? logger = requiresRecycle ? LogContext.Error : LogContext.Warning;

                        logger?.Log(args.Exception,
                            "Exception on Receiver {InputAddress} during {Action} ActiveDispatchCount({activeDispatch}) ErrorRequiresRecycle({requiresRecycle})",
                            _clientContext.InputAddress, args.ErrorSource, ActiveDispatchCount, requiresRecycle);
                    }

                    break;
                }
            }

            if (requiresRecycle)
            {
                await _clientContext.NotifyFaulted(args.Exception, args.EntityPath).ConfigureAwait(false);

                TrySetConsumeException(args.Exception);
            }
        }

        protected override async Task ActiveAndActualAgentsCompleted(StopContext context)
        {
            await _clientContext.ShutdownAsync().ConfigureAwait(false);

            await base.ActiveAndActualAgentsCompleted(context).ConfigureAwait(false);

            try
            {
                await _clientContext.CloseAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                LogContext.Warning?.Log(exception, "Failed to close the message receiver context: {InputAddress}", _clientContext.InputAddress);
            }
        }

        async Task OnMessage(ProcessMessageEventArgs messageReceiver, ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            if (IsStopping)
                return;

            MessageLockContext lockContext = new ServiceBusMessageLockContext(messageReceiver, message);
            var context = new ServiceBusReceiveContext(message, _context, lockContext, _clientContext);

            CancellationTokenSource cancellationTokenSource = null;
            CancellationTokenRegistration timeoutRegistration = default;
            CancellationTokenRegistration registration = default;
            if (cancellationToken.CanBeCanceled)
            {
                void Callback()
                {
                    if (_context.ConsumerStopTimeout.HasValue)
                    {
                        cancellationTokenSource = new CancellationTokenSource(_context.ConsumerStopTimeout.Value);
                        timeoutRegistration = cancellationTokenSource.Token.Register(context.Cancel);
                    }
                    else
                        context.Cancel();
                }

                registration = cancellationToken.Register(Callback);
            }


            try
            {
                await Dispatch(message, context, lockContext).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // do NOT let exceptions propagate to the Azure SDK
            }
            finally
            {
                timeoutRegistration.Dispose();
                registration.Dispose();

                cancellationTokenSource?.Dispose();

                context.Dispose();
            }
        }

        protected async Task Dispatch(ServiceBusReceivedMessage message, ServiceBusReceiveContext context, MessageLockContext lockContext)
        {
            try
            {
                await Dispatch(context.SequenceNumber, context, new ServiceBusReceiveLockContext(_context.InputAddress, lockContext, message))
                    .ConfigureAwait(false);
            }
            catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.SessionLockLost)
            {
                LogContext.Error?.Log("Session Lock Lost: {InputAddress} {MessageId} {SequenceNumber} ({SessionId})", _context.InputAddress,
                    message.MessageId, message.SequenceNumber, message.SessionId);

                await _context.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);
                throw;
            }
            catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.MessageLockLost)
            {
                LogContext.Error?.Log("Message Lock Lost: {InputAddress} {MessageId} {SequenceNumber}", _context.InputAddress, message.MessageId,
                    message.SequenceNumber);

                await _context.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);
                throw;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                context.LogTransportFaulted(exception);
                throw;
            }
        }
    }
}
