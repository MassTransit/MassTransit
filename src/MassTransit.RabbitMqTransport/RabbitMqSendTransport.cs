// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using MassTransit.Pipeline;
    using Pipeline;
    using RabbitMQ.Client;
    using Subscriptions;
    using Transports;


    public class RabbitMqSendTransport :
        ISendTransport
    {
        readonly IModelCache _modelCache;
        readonly Connectable<ISendObserver> _observers;
        readonly SendSettings _sendSettings;

        public RabbitMqSendTransport(IModelCache modelCache, SendSettings sendSettings)
        {
            _observers = new Connectable<ISendObserver>();
            _sendSettings = sendSettings;
            _modelCache = modelCache;
        }

        Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            IPipe<ModelContext> modelPipe =
                Pipe.New<ModelContext>(p => p.ExecuteAsync(modelContext => SendMessage(message, pipe, cancelSend)));

            return _modelCache.Send(modelPipe, cancelSend);
        }

        public Task Move(ReceiveContext context)
        {
            throw new NotImplementedException();
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
                p.Filter(new PrepareSendExchangeFilter(_sendSettings));

                p.ExecuteAsync(async modelContext =>
                {
                    IBasicProperties properties = modelContext.Model.CreateBasicProperties();
                    properties.Headers = new Dictionary<string, object>();

                    var context = new RabbitMqSendContextImpl<T>(properties, message, _sendSettings, cancelSend);

                    try
                    {
                        await pipe.Send(context);

                        properties.Headers["Content-Type"] = context.ContentType.ToString();

                        properties.SetPersistent(context.Durable);

                        if (context.MessageId.HasValue)
                            properties.MessageId = context.MessageId.ToString();

                        if (context.CorrelationId.HasValue)
                            properties.CorrelationId = context.CorrelationId.ToString();

                        if (context.TimeToLive.HasValue)
                            properties.Expiration = context.TimeToLive.Value.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);

                        await _observers.ForEach(x => x.PreSend(context));

                        await
                            modelContext.Model.BasicPublishAsync(context.Exchange, context.RoutingKey, context.Mandatory,
                                context.Immediate,
                                context.BasicProperties, context.Body);

                        await _observers.ForEach(x => x.PostSend(context));
                    }
                    catch (Exception ex)
                    {
                        _observers.ForEach(x => x.SendFault(context, ex))
                            .Wait(cancelSend);

                        throw;
                    }
                });
            });

            await _modelCache.Send(modelPipe, cancelSend);
        }
    }
}