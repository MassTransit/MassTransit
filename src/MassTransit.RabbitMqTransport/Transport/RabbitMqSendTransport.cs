// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Internals.Extensions;
    using Logging;
    using MassTransit.Pipeline.Observables;
    using Transports;


    public class RabbitMqSendTransport :
        Supervisor,
        ISendTransport
    {
        static readonly ILog _log = Logger.Get<RabbitMqSendTransport>();

        readonly string _exchange;
        readonly IFilter<ModelContext> _filter;
        readonly IPipeContextSource<ModelContext> _modelSource;
        readonly SendObservable _observers;

        public RabbitMqSendTransport(IAgent<ModelContext> modelSource, IFilter<ModelContext> preSendFilter, string exchange)
        {
            _modelSource = modelSource;
            _filter = preSendFilter;
            _exchange = exchange;

            _observers = new SendObservable();

            Add(modelSource);
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            if (IsStopped)
                throw new TransportUnavailableException($"The RabbitMQ send transport is stopped: {_exchange}");

            IPipe<ModelContext> modelPipe = Pipe.New<ModelContext>(p =>
            {
                p.UseFilter(_filter);

                p.UseExecuteAsync(async modelContext =>
                {
                    var properties = modelContext.Model.CreateBasicProperties();

                    var context = new BasicPublishRabbitMqSendContext<T>(properties, _exchange, message, cancellationToken);
                    try
                    {
                        await pipe.Send(context).ConfigureAwait(false);

                        var body = context.Body;

                        if (context.TryGetPayload(out PublishContext publishContext))
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

                        var publishTask = modelContext.BasicPublishAsync(context.Exchange, context.RoutingKey ?? "", context.Mandatory,
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

            await _modelSource.Send(modelPipe, cancellationToken).ConfigureAwait(false);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        protected override Task StopSupervisor(StopSupervisorContext context)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Stopping transport: {0}", _exchange);

            return base.StopSupervisor(context);
        }
    }
}