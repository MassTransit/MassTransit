namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Fabric;
    using Initializers.TypeConverters;
    using Logging;
    using Transports;


    /// <summary>
    /// Support in-memory message queue that is not durable, but supports parallel delivery of messages based on TPL usage.
    /// </summary>
    public class InMemorySendTransport :
        ISendTransport
    {
        static readonly DateTimeOffsetTypeConverter _dateTimeOffsetConverter = new DateTimeOffsetTypeConverter();
        static readonly DateTimeTypeConverter _dateTimeConverter = new DateTimeTypeConverter();
        readonly InMemorySendTransportContext _context;

        public InMemorySendTransport(InMemorySendTransportContext context)
        {
            _context = context;
        }

        public async Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            var context = new MessageSendContext<T>(message, cancellationToken);

            await pipe.Send(context).ConfigureAwait(false);

            StartedActivity? activity = LogContext.IfEnabled(_context.ActivityName)?.StartSendActivity(context);
            try
            {
                if (_context.SendObservers.Count > 0)
                    await _context.SendObservers.PreSend(context).ConfigureAwait(false);

                var messageId = context.MessageId ?? NewId.NextGuid();

                var transportMessage = new InMemoryTransportMessage(messageId, context.Body.GetBytes(), context.ContentType.ToString(),
                    TypeCache<T>.ShortName) { Delay = context.Delay };

                SetHeaders(transportMessage.Headers, context.Headers);

                await _context.Exchange.Send(transportMessage, cancellationToken).ConfigureAwait(false);

                activity?.Update(context);
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

        static void SetHeaders(IDictionary<string, object> dictionary, SendHeaders headers)
        {
            foreach (KeyValuePair<string, object> header in headers.GetAll())
            {
                if (header.Value == null)
                {
                    if (dictionary.ContainsKey(header.Key))
                        dictionary.Remove(header.Key);

                    continue;
                }

                if (dictionary.ContainsKey(header.Key))
                    continue;

                switch (header.Value)
                {
                    case DateTimeOffset value:
                        if (_dateTimeOffsetConverter.TryConvert(value, out string text))
                            dictionary[header.Key] = text;
                        break;

                    case DateTime value:
                        if (_dateTimeConverter.TryConvert(value, out text))
                            dictionary[header.Key] = text;
                        break;

                    case Uri value:
                        dictionary[header.Key] = value.ToString();
                        break;

                    case string value:
                        dictionary[header.Key] = value;
                        break;

                    case bool value when value:
                        dictionary[header.Key] = bool.TrueString;
                        break;

                    case IFormattable formatValue:
                        if (header.Value.GetType().IsValueType)
                            dictionary[header.Key] = header.Value;
                        else
                            dictionary[header.Key] = formatValue.ToString();
                        break;
                }
            }
        }
    }
}
