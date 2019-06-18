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
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Pipes;
    using Integration;
    using Internals.Extensions;
    using Logging;
    using MassTransit.Pipeline.Observables;
    using Transports;


    public class RabbitMqSendTransport :
        Supervisor,
        ISendTransport,
        IAsyncDisposable
    {
        static readonly ILog _log = Logger.Get<RabbitMqSendTransport>();

        readonly string _exchange;
        readonly IFilter<ModelContext> _filter;
        readonly IModelContextSupervisor _modelContextSupervisor;
        readonly SendObservable _observers;

        public RabbitMqSendTransport(IModelContextSupervisor modelContextSupervisor, IFilter<ModelContext> preSendFilter, string exchange)
        {
            _modelContextSupervisor = modelContextSupervisor;
            _filter = preSendFilter;
            _exchange = exchange;

            _observers = new SendObservable();

            Add(modelContextSupervisor);
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            return this.Stop("Disposed", cancellationToken);
        }

        Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            if (IsStopped)
                throw new TransportUnavailableException($"The RabbitMQ send transport is stopped: {_exchange}");

            var sendPipe = new SendPipe<T>(_filter, _observers, _exchange, message, pipe, cancellationToken);

            return _modelContextSupervisor.Send(sendPipe, cancellationToken);
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


        struct SendPipe<T> :
            IPipe<ModelContext>
            where T : class
        {
            readonly IFilter<ModelContext> _filter;
            readonly SendObservable _observers;
            readonly string _exchange;
            readonly T _message;
            readonly IPipe<SendContext<T>> _pipe;
            readonly CancellationToken _cancellationToken;

            public SendPipe(IFilter<ModelContext> filter, SendObservable observers, string exchange, T message, IPipe<SendContext<T>> pipe,
                CancellationToken cancellationToken)
            {
                _filter = filter;
                _observers = observers;
                _exchange = exchange;
                _message = message;
                _pipe = pipe;
                _cancellationToken = cancellationToken;
            }

            public async Task Send(ModelContext modelContext)
            {
                await _filter.Send(modelContext, Pipe.Empty<ModelContext>()).ConfigureAwait(false);

                var properties = modelContext.Model.CreateBasicProperties();

                var context = new BasicPublishRabbitMqSendContext<T>(properties, _exchange, _message, _cancellationToken);
                try
                {
                    await _pipe.Send(context).ConfigureAwait(false);

                    byte[] body = context.Body;

                    if (context.TryGetPayload(out PublishContext publishContext))
                        context.Mandatory = context.Mandatory || publishContext.Mandatory;

                    if (properties.Headers == null)
                        properties.Headers = new Dictionary<string, object>();

                    properties.ContentType = context.ContentType.MediaType;

                    properties.Headers["Content-Type"] = context.ContentType.MediaType;

                    properties.Headers.SetTextHeaders(context.Headers, (_, text) => text);

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
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
