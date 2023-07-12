namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Context;
    using Transports;


    public class ServiceBusMessageReceiver :
        IServiceBusMessageReceiver
    {
        readonly ReceiveEndpointContext _context;
        readonly IReceivePipeDispatcher _dispatcher;

        public ServiceBusMessageReceiver(ReceiveEndpointContext context)
        {
            _context = context;

            _dispatcher = context.CreateReceivePipeDispatcher();
        }

        public async Task Handle(ServiceBusReceivedMessage message, CancellationToken cancellationToken)
        {
            var context = new ServiceBusReceiveContext(message, _context);

            CancellationTokenRegistration registration = default;
            if (cancellationToken.CanBeCanceled)
                registration = cancellationToken.Register(context.Cancel);

            try
            {
                await _dispatcher.Dispatch(context, NoLockReceiveContext.Instance).ConfigureAwait(false);
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
            finally
            {
                registration.Dispose();
                context.Dispose();
            }
        }
    }
}
