// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading;
    using System.Threading.Tasks;
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
        readonly IList<IFilter<ModelContext>> _filters;

        readonly IModelCache _modelCache;
        readonly Connectable<ISendObserver> _observers;
        readonly SendSettings _sendSettings;

        public RabbitMqSendTransport(IModelCache modelCache, SendSettings sendSettings, params ExchangeBindingSettings[] exchangeBindings)
        {
            _observers = new Connectable<ISendObserver>();
            _sendSettings = sendSettings;
            _modelCache = modelCache;

            _filters = new List<IFilter<ModelContext>>();

            _filters.Add(new PrepareSendExchangeFilter(_sendSettings));
            foreach (ExchangeBindingSettings binding in exchangeBindings)
                _filters.Add(new SendExchangeBindingModelFilter(binding));
        }

        Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            return SendMessage(message, pipe, cancelSend);
        }

        async Task ISendTransport.Move(ReceiveContext context, IPipe<SendContext> pipe)
        {
            IPipe<ModelContext> modelPipe = Pipe.New<ModelContext>(p =>
            {
                foreach (var filter in _filters)
                    p.Filter(filter);

                p.ExecuteAsync(async modelContext =>
                {
                    Guid? messageId = context.TransportHeaders.Get("MessageId", default(Guid?));

                    IBasicProperties properties = modelContext.Model.CreateBasicProperties();
                    properties.Headers = new Dictionary<string, object>();

                    RabbitMqBasicConsumeContext basicConsumeContext;
                    if (context.TryGetPayload(out basicConsumeContext))
                    {
                        if (basicConsumeContext.Properties.IsMessageIdPresent())
                        {
                            Guid transportMessageId;
                            if (Guid.TryParse(basicConsumeContext.Properties.MessageId, out transportMessageId))
                                messageId = transportMessageId;

                            properties.MessageId = basicConsumeContext.Properties.MessageId;
                        }
                        properties.DeliveryMode = basicConsumeContext.Properties.IsDeliveryModePresent()
                            ? basicConsumeContext.Properties.DeliveryMode
                            : (byte)2;

                        if (basicConsumeContext.Properties.IsContentTypePresent())
                            properties.ContentType = basicConsumeContext.Properties.ContentType;

                        if (basicConsumeContext.Properties.IsCorrelationIdPresent())
                            properties.CorrelationId = basicConsumeContext.Properties.CorrelationId;

                        if (basicConsumeContext.Properties.IsExpirationPresent())
                            properties.Expiration = basicConsumeContext.Properties.Expiration;
                    }

                    try
                    {
                        properties.Headers["Content-Type"] = context.ContentType.MediaType;

                        properties.SetPersistent(_sendSettings.Durable);

                        if (messageId.HasValue)
                            properties.MessageId = messageId.ToString();

                        byte[] body;
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var bodyStream = context.GetBody())
                            {
                                bodyStream.CopyTo(memoryStream);
                            }

                            body = memoryStream.ToArray();
                        }

                        Task task = modelContext.BasicPublishAsync(_sendSettings.ExchangeName, "", true, false, properties, body);
                        context.AddPendingTask(task);

                        if (_log.IsDebugEnabled)
                        {
                            _log.DebugFormat("MOVE {0} ({1} to {2})", messageId.HasValue ? messageId.Value.ToString() : "N/A", context.InputAddress,
                                modelContext.ConnectionContext.HostSettings.GetSendAddress(_sendSettings));
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

        public ConnectHandle Connect(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        async Task SendMessage<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
            where T : class
        {
            IPipe<ModelContext> modelPipe = Pipe.New<ModelContext>(p =>
            {
                foreach (var filter in _filters)
                    p.Filter(filter);

                p.ExecuteAsync(async modelContext =>
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Sending {0} to {1}", TypeMetadataCache<T>.ShortName, _sendSettings.ExchangeName);

                    IBasicProperties properties = modelContext.Model.CreateBasicProperties();
                    properties.Headers = new Dictionary<string, object>();

                    var context = new RabbitMqSendContextImpl<T>(properties, message, _sendSettings, cancelSend);

                    try
                    {
                        await pipe.Send(context).ConfigureAwait(false);

                        properties.Headers["Content-Type"] = context.ContentType.MediaType;

                        properties.SetPersistent(context.Durable);

                        if (context.MessageId.HasValue)
                            properties.MessageId = context.MessageId.ToString();

                        if (context.CorrelationId.HasValue)
                            properties.CorrelationId = context.CorrelationId.ToString();

                        if (context.TimeToLive.HasValue)
                            properties.Expiration = context.TimeToLive.Value.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);

                        await _observers.ForEach(x => x.PreSend(context)).ConfigureAwait(false);

                        await modelContext.BasicPublishAsync(context.Exchange, context.RoutingKey, context.Mandatory,
                            context.Immediate, context.BasicProperties, context.Body).ConfigureAwait(false);

                        context.DestinationAddress.LogSent(context.MessageId.HasValue ? context.MessageId.Value.ToString("N") : "",
                            TypeMetadataCache<T>.ShortName);

                        await _observers.ForEach(x => x.PostSend(context)).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _observers.ForEach(x => x.SendFault(context, ex));

                        if (_log.IsErrorEnabled)
                            _log.Error("Send Fault: " + context.DestinationAddress, ex);

                        throw;
                    }
                });
            });

            await _modelCache.Send(modelPipe, cancelSend).ConfigureAwait(false);
        }
    }
}