namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Initializers.TypeConverters;
    using Internals.Extensions;
    using Logging;
    using RabbitMQ.Client;
    using Transports;


    public class RabbitMqSendTransport :
        Supervisor,
        ISendTransport,
        IAsyncDisposable
    {
        readonly RabbitMqSendTransportContext _context;

        public RabbitMqSendTransport(RabbitMqSendTransportContext context)
        {
            _context = context;

            Add(context.ModelContextSupervisor);
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            return this.Stop("Disposed", cancellationToken);
        }

        Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            if (IsStopped)
                throw new TransportUnavailableException($"The RabbitMQ send transport is stopped: {_context.Exchange}");

            var sendPipe = new SendPipe<T>(_context, message, pipe, cancellationToken);

            return _context.ModelContextSupervisor.Send(sendPipe, cancellationToken);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        protected override Task StopSupervisor(StopSupervisorContext context)
        {
            LogContext.Debug?.Log("Stopping send transport: {Exchange}", _context.Exchange);

            return base.StopSupervisor(context);
        }


        struct SendPipe<T> :
            IPipe<ModelContext>
            where T : class
        {
            readonly RabbitMqSendTransportContext _context;
            readonly T _message;
            readonly IPipe<SendContext<T>> _pipe;
            readonly CancellationToken _cancellationToken;

            public SendPipe(RabbitMqSendTransportContext context, T message, IPipe<SendContext<T>> pipe, CancellationToken
                cancellationToken)
            {
                _context = context;
                _message = message;
                _pipe = pipe;
                _cancellationToken = cancellationToken;
            }

            public async Task Send(ModelContext modelContext)
            {
                LogContext.SetCurrentIfNull(_context.LogContext);

                await _context.ConfigureTopologyPipe.Send(modelContext).ConfigureAwait(false);

                var properties = modelContext.Model.CreateBasicProperties();

                var context = new BasicPublishRabbitMqSendContext<T>(properties, _context.Exchange, _message, _cancellationToken);

                var activity = LogContext.IfEnabled(OperationName.Transport.Send)?.StartActivity(new {_context.Exchange});
                try
                {
                    await _pipe.Send(context).ConfigureAwait(false);

                    activity.AddSendContextHeaders(context);

                    byte[] body = context.Body;

                    if (context.TryGetPayload(out PublishContext publishContext))
                        context.Mandatory = context.Mandatory || publishContext.Mandatory;

                    if (properties.Headers == null)
                        properties.Headers = new Dictionary<string, object>();

                    properties.ContentType = context.ContentType.MediaType;

                    properties.Headers["Content-Type"] = context.ContentType.MediaType;

                    SetHeaders(properties.Headers, context.Headers);

                    properties.Persistent = context.Durable;

                    if (context.MessageId.HasValue)
                        properties.MessageId = context.MessageId.ToString();

                    if (context.CorrelationId.HasValue)
                        properties.CorrelationId = context.CorrelationId.ToString();

                    if (context.TimeToLive.HasValue)
                        properties.Expiration = context.TimeToLive.Value.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);

                    if (_context.SendObservers.Count > 0)
                        await _context.SendObservers.PreSend(context).ConfigureAwait(false);

                    var publishTask = modelContext.BasicPublishAsync(context.Exchange, context.RoutingKey ?? "", context.Mandatory,
                        context.BasicProperties, body, context.AwaitAck);

                    await publishTask.OrCanceled(context.CancellationToken).ConfigureAwait(false);

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

            public void Probe(ProbeContext context)
            {
            }

            void SetHeaders(IDictionary<string, object> dictionary, SendHeaders headers)
            {
                foreach (var header in headers.GetAll())
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
                            if (_dateTimeOffsetConverter.TryConvert(value, out long result))
                                dictionary[header.Key] = new AmqpTimestamp(result);
                            else if (_dateTimeOffsetConverter.TryConvert(value, out string text))
                                dictionary[header.Key] = text;

                            break;

                        case DateTime value:
                            if (_dateTimeConverter.TryConvert(value, out result))
                                dictionary[header.Key] = new AmqpTimestamp(result);
                            else if (_dateTimeConverter.TryConvert(value, out string text))
                                dictionary[header.Key] = text;

                            break;

                        case string value:
                            dictionary[header.Key] = value;
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


        static readonly DateTimeOffsetTypeConverter _dateTimeOffsetConverter = new DateTimeOffsetTypeConverter();
        static readonly DateTimeTypeConverter _dateTimeConverter = new DateTimeTypeConverter();
    }
}
