namespace MassTransit.Transports.InMemory
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Fabric;
    using GreenPipes;
    using Logging;
    using Metadata;


    /// <summary>
    /// Support in-memory message queue that is not durable, but supports parallel delivery of messages
    /// based on TPL usage.
    /// </summary>
    public class InMemorySendTransport :
        ISendTransport
    {
        readonly InMemorySendTransportContext _context;

        public InMemorySendTransport(InMemorySendTransportContext context)
        {
            _context = context;
        }

        public string ExchangeName => _context.Exchange.Name;

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            var context = new InMemorySendContext<T>(message, cancellationToken);

            var activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartActivity(new {Exchange = ExchangeName});
            try
            {
                await pipe.Send(context).ConfigureAwait(false);

                activity.AddSendContextHeaders(context);

                var messageId = context.MessageId ?? NewId.NextGuid();

                if (_context.SendObservers.Count > 0)
                    await _context.SendObservers.PreSend(context).ConfigureAwait(false);

                var transportMessage = new InMemoryTransportMessage(messageId, context.Body, context.ContentType.MediaType, TypeMetadataCache<T>.ShortName);

                await _context.Exchange.Send(transportMessage).ConfigureAwait(false);

                context.LogSent();

                if (_context.SendObservers.Count > 0)
                    await _context.SendObservers.PostSend(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                context.LogFaulted(ex);

                if (_context.SendObservers.Count > 0)
                    await _context.SendObservers.SendFault(context, ex).ConfigureAwait(false);

                throw;
            }
            finally
            {
                activity?.Stop();
            }
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }
    }
}
