using GreenPipes;
using MassTransit.Context;
using MassTransit.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public class OnRampSendTransport : ISendTransport
    {
        private readonly IOnRampSendTransportContext _context;

        public OnRampSendTransport(IOnRampSendTransportContext context)
        {
            _context = context;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer) => _context.ConnectSendObserver(observer);

        public async Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken) where T : class
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            // check the context destination address here, versus the destination passed in with _context
            var context = new MessageSendContext<T>(message, cancellationToken);

            await pipe.Send(context).ConfigureAwait(false);

            if(!context.TryGetPayload(out IOnRampTransportRepository repository))
            {
                throw new OnRampException("Couldn't resolve onramp repository. Configuration is incorrect.");
            }

            if(context.RequestId.GetValueOrDefault() != default)
            {
                await _context.RealSendTransport.Send(message, pipe, cancellationToken).ConfigureAwait(false);
                return; // short circuit, we called the real transport
            }
            else if (context.Headers.TryGetHeader("MT-OnRamp-Sweeper", out var _))
            {
                await _context.RealSendTransport.Send(message, pipe, cancellationToken).ConfigureAwait(false);
                return; // short circuit, because this is the sweeper trying to send the message
            }

            StartedActivity? activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartSendActivity(context);
            try
            {
                if (_context.SendObservers.Count > 0)
                    await _context.SendObservers.PreSend(context).ConfigureAwait(false);

                var outboxMessage = await CreateOutboxMessage(context).ConfigureAwait(false);

                // We don't call save changes here, we let the caller (most likely
                // a scoped HTTP Request) perform the save changes async (and/or commit
                // the transaction).
                await repository.InsertMessage(outboxMessage).ConfigureAwait(false);

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

        static async Task<JsonSerializedMessage> CreateOutboxMessage<T>(MessageSendContext<T> context) where T : class
        {
            var messageId = context.MessageId ?? NewId.NextGuid();

            var body = Encoding.UTF8.GetString(context.Body);

            var mediaType = context.ContentType.MediaType;

            var serializedMessage = new JsonSerializedMessage
            {
                MessageId = messageId.ToString(),
                Body = body,
                ContentType = mediaType,
                ConversationId = context.ConversationId?.ToString(),
                CorrelationId = context.CorrelationId?.ToString(),
                Destination = context.DestinationAddress,
                FaultAddress = context.FaultAddress?.ToString(),
                InitiatorId = context.InitiatorId?.ToString(),
                RequestId = context.RequestId?.ToString(),
                ResponseAddress = context.ResponseAddress?.ToString()
            };

            IEnumerable<KeyValuePair<string, object>> headers = context.Headers.GetAll();
            if (headers.Any())
                serializedMessage.HeadersAsJson = JsonConvert.SerializeObject(headers);

            return serializedMessage;
        }
    }
}
