// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Reflection;
    using Context;
    using Contexts;
    using GreenPipes;
    using Integration;
    using Internals.Extensions;
    using Logging;
    using MassTransit.Pipeline.Observables;
    using RabbitMQ.Client;
    using Serialization;
    using Transports;


    public class RabbitMqSendTransport :
        ISendTransport
    {
        static readonly ILog _log = Logger.Get<RabbitMqSendTransport>();

        readonly IFilter<ModelContext> _filter;
        readonly IModelCache _modelCache;
        readonly SendObservable _observers;
        readonly string _exchange;

        public RabbitMqSendTransport(IModelCache modelCache, IFilter<ModelContext> preSendFilter, string exchange)
        {
            _observers = new SendObservable();
            _modelCache = modelCache;

            _filter = preSendFilter;
            _exchange = exchange;
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            IPipe<ModelContext> modelPipe = Pipe.New<ModelContext>(p =>
            {
                p.UseFilter(_filter);

                p.UseExecuteAsync(async modelContext =>
                {
                    var properties = modelContext.Model.CreateBasicProperties();

                    var context = new BasicPublishRabbitMqSendContext<T>(properties, _exchange, message, cancelSend);

                    try
                    {
                        await pipe.Send(context).ConfigureAwait(false);

                        var body = context.Body;

                        PublishContext publishContext;
                        if (context.TryGetPayload(out publishContext))
                            context.Mandatory = context.Mandatory || publishContext.Mandatory;

                        properties.ContentType = context.ContentType.MediaType;

                        KeyValuePair<string, object>[] headers = context.Headers.GetAll()
                            .Where(x => x.Value != null && (x.Value is string || x.Value.GetType().GetTypeInfo().IsValueType))
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

                        var publishTask = modelContext.BasicPublishAsync(context.Exchange, context.RoutingKey, context.Mandatory,
                            context.BasicProperties, body, context.AwaitAck);
                        
                        await publishTask.WithCancellation(context.CancellationToken).ConfigureAwait(false);

                        context.LogSent();

                        await _observers.PostSend(context).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        context.LogFaulted(ex);

                        await _observers.SendFault(context, ex).ConfigureAwait(false);

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
                    try
                    {
                        IBasicProperties properties;
                        string routingKey = "";

                        RabbitMqBasicConsumeContext basicConsumeContext;
                        if (context.TryGetPayload(out basicConsumeContext))
                        {
                            properties = basicConsumeContext.Properties;
                            routingKey = basicConsumeContext.RoutingKey;
                        }
                        else
                        {
                            properties = modelContext.Model.CreateBasicProperties();
                            properties.Headers = new Dictionary<string, object>();
                        }

                        var moveContext = new RabbitMqMoveContext(context, properties, _exchange, routingKey ?? "");

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

                        var task = modelContext.BasicPublishAsync(_exchange, "", true, properties, body, true);
                        context.AddPendingTask(task);
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsErrorEnabled)
                            _log.Error("Faulted moving message to error queue: " + _exchange, ex);

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

            public RabbitMqMoveContext(ReceiveContext context, IBasicProperties properties, string exchange, string routingKey)
            {
                _context = context;
                BasicProperties = properties;
                Exchange = exchange;
                RoutingKey = routingKey;
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
            public Guid? ScheduledMessageId { get; set; }

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
        }
    }
}