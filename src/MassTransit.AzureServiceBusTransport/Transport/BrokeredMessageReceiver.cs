namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Logging;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;
    using Transports;


    /// <summary>
    /// Receives a <see cref="BrokeredMessage"/>.
    /// </summary>
    public class BrokeredMessageReceiver :
        IBrokeredMessageReceiver
    {
        readonly Uri _inputAddress;
        readonly ReceiveEndpointContext _context;

        public BrokeredMessageReceiver(Uri inputAddress, ReceiveEndpointContext context)
        {
            _inputAddress = inputAddress;
            _context = context;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("receiver");
            scope.Add("type", "brokeredMessage");
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _context.ConnectReceiveObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        async Task IBrokeredMessageReceiver.Handle(BrokeredMessage message, Action<ReceiveContext> contextCallback)
        {
            LogContext.Current = _context.LogContext;

            var context = new ServiceBusReceiveContext(_inputAddress, message, _context);
            contextCallback?.Invoke(context);

            var activity = LogContext.IfEnabled(OperationName.Transport.Receive)?.StartActivity();
            activity.AddReceiveContextHeaders(context);

            try
            {
                if (_context.ReceiveObservers.Count > 0)
                    await _context.ReceiveObservers.PreReceive(context).ConfigureAwait(false);

                if (message.LockedUntilUtc <= DateTime.UtcNow)
                    throw new MessageLockExpiredException(_inputAddress, $"The message lock expired: {message.MessageId}");

                if (message.ExpiresAtUtc < DateTime.UtcNow)
                    throw new MessageTimeToLiveExpiredException(_inputAddress, $"The message TTL expired: {message.MessageId}");

                await _context.ReceivePipe.Send(context).ConfigureAwait(false);

                await context.ReceiveCompleted.ConfigureAwait(false);

                await message.CompleteAsync().ConfigureAwait(false);

                if (_context.ReceiveObservers.Count > 0)
                    await _context.ReceiveObservers.PostReceive(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                try
                {
                    await message.AbandonAsync().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    LogContext.Warning?.Log(exception, "Abandon message faulted: {MessageId", message.MessageId);
                }

                if (_context.ReceiveObservers.Count > 0)
                    await _context.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);
            }
            finally
            {
                activity?.Stop();

                context.Dispose();
            }
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _context.ReceivePipe.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _context.ReceivePipe.ConnectConsumeObserver(observer);
        }
    }
}
