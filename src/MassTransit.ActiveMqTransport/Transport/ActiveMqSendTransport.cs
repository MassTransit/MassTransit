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
namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Apache.NMS.Util;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Internals.Extensions;
    using Logging;
    using MassTransit.Pipeline.Observables;
    using Transports;


    public class ActiveMqSendTransport :
        Supervisor,
        ISendTransport
    {
        static readonly ILog _log = Logger.Get<ActiveMqSendTransport>();

        readonly DestinationType _destinationType;
        readonly string _entityName;
        readonly IFilter<SessionContext> _filter;
        readonly ISource<SessionContext> _modelSource;
        readonly SendObservable _observers;

        public ActiveMqSendTransport(IAgent<SessionContext> modelSource, IFilter<SessionContext> preSendFilter, string entityName,
            DestinationType destinationType)
        {
            _modelSource = modelSource;
            _filter = preSendFilter;
            _entityName = entityName;
            _destinationType = destinationType;

            _observers = new SendObservable();

            Add(modelSource);
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            if (IsStopped)
                throw new TransportUnavailableException($"The RabbitMQ send transport is stopped: {_entityName}");

            IPipe<SessionContext> modelPipe = Pipe.New<SessionContext>(p =>
            {
                p.UseFilter(_filter);

                p.UseExecuteAsync(async modelContext =>
                {
                    var producer = modelContext.GetOrAddPayload(() => CreateMessageProducer(modelContext));

                    var context = new TransportActiveMqSendContext<T>(message, cancelSend);
                    try
                    {
                        await pipe.Send(context).ConfigureAwait(false);

                        byte[] body = context.Body;

                        var transportMessage = modelContext.Session.CreateBytesMessage();

                        KeyValuePair<string, object>[] headers = context.Headers.GetAll()
                            .Where(x => x.Value != null && (x.Value is string || x.Value.GetType().GetTypeInfo().IsValueType))
                            .ToArray();

                        foreach (KeyValuePair<string, object> header in headers)
                        {
                            if (transportMessage.Properties.Contains(header.Key))
                                continue;

                            transportMessage.Properties[header.Key] = header.Value;
                        }

                        transportMessage.Properties["Content-Type"] = context.ContentType.MediaType;

                        transportMessage.NMSDeliveryMode = context.Durable ? MsgDeliveryMode.Persistent : MsgDeliveryMode.NonPersistent;

                        if (context.MessageId.HasValue)
                            transportMessage.NMSMessageId = context.MessageId.ToString();

                        if (context.CorrelationId.HasValue)
                            transportMessage.NMSCorrelationID = context.CorrelationId.ToString();

                        if (context.TimeToLive.HasValue)
                            transportMessage.NMSTimeToLive = context.TimeToLive.Value;

                        if (context.Priority.HasValue)
                            transportMessage.NMSPriority = context.Priority.Value;

                        transportMessage.Content = body;

                        await _observers.PreSend(context).ConfigureAwait(false);

                        var publishTask = Task.Run(() => producer.Send(transportMessage), context.CancellationToken);

                        await publishTask.UntilCompletedOrCanceled(context.CancellationToken).ConfigureAwait(false);


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

            await _modelSource.Send(modelPipe, cancelSend).ConfigureAwait(false);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        IMessageProducer CreateMessageProducer(SessionContext sessionContext)
        {
            var destination = SessionUtil.GetDestination(sessionContext.Session, _entityName, _destinationType);

            return sessionContext.Session.CreateProducer(destination);
        }

        protected override Task StopSupervisor(StopSupervisorContext context)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Stopping transport: {0}", _entityName);

            return base.StopSupervisor(context);
        }
    }
}