// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Integration;
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using RabbitMQ.Client;
    using Topology;
    using Transports;
    using Util;


    public class RabbitMqSendTransport :
        ISendTransport
    {
        static readonly ILog _log = Logger.Get<RabbitMqSendTransport>();
        readonly PrepareSendExchangeFilter _filter;
        readonly IModelCache _modelCache;
        readonly SendObservable _observers;
        readonly SendSettings _sendSettings;

        public RabbitMqSendTransport(IModelCache modelCache, SendSettings sendSettings, params ExchangeBindingSettings[] exchangeBindings)
        {
            _observers = new SendObservable();
            _sendSettings = sendSettings;
            _modelCache = modelCache;

            _filter = new PrepareSendExchangeFilter(_sendSettings, exchangeBindings);
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            IPipe<ModelContext> modelPipe = Pipe.New<ModelContext>(p =>
            {
                p.UseFilter(_filter);

                p.UseExecuteAsync(async modelContext =>
                {
                    var properties = modelContext.Model.CreateBasicProperties();

                    var context = new RabbitMqSendContextImpl<T>(properties, message, _sendSettings, cancelSend);

                    try
                    {
                        await pipe.Send(context).ConfigureAwait(false);

                        properties.ContentType = context.ContentType.MediaType;

                        KeyValuePair<string, object>[] headers = context.Headers.GetAll()
                            .Where(x => x.Value != null && (x.Value is string || x.Value.GetType().IsValueType))
                            .ToArray();

                        if (properties.Headers == null)
                            properties.Headers = new Dictionary<string, object>(headers.Length);

                        foreach (KeyValuePair<string, object> header in headers)
                        {
                            if (properties.Headers.ContainsKey(header.Key))
                                continue;

                            properties.SetHeader(header.Key, header.Value);
                        }

                        properties.Headers["Content-Type"] = context.ContentType.MediaType;

                        properties.Persistent = context.Durable;

                        if (context.MessageId.HasValue)
                            properties.MessageId = context.MessageId.ToString();

                        if (context.CorrelationId.HasValue)
                            properties.CorrelationId = context.CorrelationId.ToString();

                        if (context.TimeToLive.HasValue)
                            properties.Expiration = context.TimeToLive.Value.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);

                        await _observers.PreSend(context).ConfigureAwait(false);

                        await modelContext.BasicPublishAsync(context.Exchange, context.RoutingKey, context.Mandatory,
                            context.BasicProperties, context.Body, context.AwaitAck).ConfigureAwait(false);

                        context.DestinationAddress.LogSent(context.MessageId?.ToString("N") ?? "", TypeMetadataCache<T>.ShortName);

                        await _observers.PostSend(context).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await _observers.SendFault(context, ex).ConfigureAwait(false);

                        if (_log.IsErrorEnabled)
                            _log.Error("Send Fault: " + context.DestinationAddress, ex);

                        throw;
                    }
                });
            });

            await _modelCache.Send(modelPipe, cancelSend).ConfigureAwait(false);
        }

        async Task ISendTransport.Move(ReceiveContext context, IPipe<SendContext> pipe)
        {
            IPipe<ModelContext> modelPipe = Pipe.New<ModelContext>(p =>
            {
                p.UseFilter(_filter);

                p.UseExecuteAsync(async modelContext =>
                {
                    Guid? messageId = context.TransportHeaders.Get("MessageId", default(Guid?));

                    try
                    {
                        IBasicProperties properties;

                        RabbitMqBasicConsumeContext basicConsumeContext;
                        if (context.TryGetPayload(out basicConsumeContext))
                            properties = basicConsumeContext.Properties;
                        else
                        {
                            properties = modelContext.Model.CreateBasicProperties();
                            properties.Headers = new Dictionary<string, object>();
                        }

                        var moveContext = new RabbitMqMoveContext(context, properties);

                        await pipe.Send(moveContext).ConfigureAwait(false);

                        byte[] body;
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var bodyStream = context.GetBody())
                            {
                                await bodyStream.CopyToAsync(memoryStream).ConfigureAwait(false);
                            }

                            body = memoryStream.ToArray();
                        }

                        var task = modelContext.BasicPublishAsync(_sendSettings.ExchangeName, "", true, properties, body, true);
                        context.AddPendingTask(task);

                        if (_log.IsDebugEnabled)
                        {
                            context.InputAddress.LogMoved(modelContext.ConnectionContext.HostSettings.GetSendAddress(_sendSettings),
                                messageId?.ToString() ?? "N/A", "Moved");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsErrorEnabled)
                            _log.Error("Move To Error Queue Fault: " + _sendSettings.ExchangeName, ex);

                        throw;
                    }
                });
            });

            await _modelCache.Send(modelPipe, context.CancellationToken).ConfigureAwait(false);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        public Task Close()
        {
            return _modelCache.Close();
        }


        class RabbitMqMoveContext :
            RabbitMqSendContext
        {
            readonly ReceiveContext _context;
            IMessageSerializer _serializer;

            public RabbitMqMoveContext(ReceiveContext context, IBasicProperties properties)
            {
                _context = context;
                BasicProperties = properties;
                AwaitAck = true;
                Headers = new RabbitMqSendHeaders(properties);
                _serializer = new CopyBodySerializer(context);
            }

            CancellationToken PipeContext.CancellationToken => _context.CancellationToken;

            bool PipeContext.HasPayloadType(Type contextType)
            {
                return _context.HasPayloadType(contextType);
            }

            bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
            {
                return _context.TryGetPayload(out payload);
            }

            TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            {
                return _context.GetOrAddPayload(payloadFactory);
            }

            public Guid? MessageId { get; set; }
            public Guid? RequestId { get; set; }
            public Guid? CorrelationId { get; set; }
            public Guid? ConversationId { get; set; }
            public Guid? InitiatorId { get; set; }
            public SendHeaders Headers { get; }
            public Uri SourceAddress { get; set; }
            public Uri DestinationAddress { get; set; }
            public Uri ResponseAddress { get; set; }
            public Uri FaultAddress { get; set; }
            public TimeSpan? TimeToLive { get; set; }
            public ContentType ContentType { get; set; }

            public IMessageSerializer Serializer
            {
                get { return _serializer; }
                set
                {
                    _serializer = value;
                    ContentType = _serializer.ContentType;
                }
            }

            SendContext<T> SendContext.CreateProxy<T>(T message)
            {
                return new SendContextProxy<T>(this, message);
            }

            public bool Durable { get; set; }
            public bool Mandatory { get; set; }
            public string Exchange { get; private set; }
            public string RoutingKey { get; set; }
            public bool AwaitAck { get; set; }

            public IBasicProperties BasicProperties { get; }


            class CopyBodySerializer : IMessageSerializer
            {
                readonly ReceiveContext _context;

                public CopyBodySerializer(ReceiveContext context)
                {
                    _context = context;
                    ContentType = context.ContentType;
                }

                public ContentType ContentType { get; }

                void IMessageSerializer.Serialize<T>(Stream stream, SendContext<T> context)
                {
                    using (var bodyStream = _context.GetBody())
                    {
                        bodyStream.CopyTo(stream);
                    }
                }
            }
        }
    }
}