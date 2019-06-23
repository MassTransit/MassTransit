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
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using MassTransit.Pipeline.Observables;
    using Transports;


    public class ActiveMqSendTransport :
        Supervisor,
        ISendTransport
    {
        readonly DestinationType _destinationType;
        readonly string _entityName;
        readonly IFilter<SessionContext> _filter;
        readonly ISessionContextSupervisor _sessionContextSupervisor;
        readonly SendObservable _observers;

        public ActiveMqSendTransport(ISessionContextSupervisor sessionContextSupervisor, IFilter<SessionContext> preSendFilter, string entityName,
            DestinationType destinationType)
        {
            _sessionContextSupervisor = sessionContextSupervisor;
            _filter = preSendFilter;
            _entityName = entityName;
            _destinationType = destinationType;

            _observers = new SendObservable();
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            if (IsStopped)
                throw new TransportUnavailableException($"The send transport is stopped: {_entityName}/{_destinationType}");

            IPipe<SessionContext> sessionPipe = Pipe.New<SessionContext>(p =>
            {
                p.UseFilter(_filter);

                p.UseExecuteAsync(async sessionContext =>
                {
                    var destination = await sessionContext.GetDestination(_entityName, _destinationType).ConfigureAwait(false);
                    var producer = await sessionContext.CreateMessageProducer(destination).ConfigureAwait(false);

                    var sendContext = new TransportActiveMqSendContext<T>(message, cancellationToken);
                    try
                    {
                        await pipe.Send(sendContext).ConfigureAwait(false);

                        byte[] body = sendContext.Body;

                        var transportMessage = sessionContext.Session.CreateBytesMessage();

                        transportMessage.Properties.SetHeaders(sendContext.Headers);

                        transportMessage.Properties["Content-Type"] = sendContext.ContentType.MediaType;

                        transportMessage.NMSDeliveryMode = sendContext.Durable ? MsgDeliveryMode.Persistent : MsgDeliveryMode.NonPersistent;

                        if (sendContext.MessageId.HasValue)
                            transportMessage.NMSMessageId = sendContext.MessageId.ToString();

                        if (sendContext.CorrelationId.HasValue)
                            transportMessage.NMSCorrelationID = sendContext.CorrelationId.ToString();

                        if (sendContext.TimeToLive.HasValue)
                            transportMessage.NMSTimeToLive = sendContext.TimeToLive.Value;

                        if (sendContext.Priority.HasValue)
                            transportMessage.NMSPriority = sendContext.Priority.Value;

                        transportMessage.Content = body;

                        await _observers.PreSend(sendContext).ConfigureAwait(false);

                        var publishTask = Task.Run(() => producer.Send(transportMessage), sendContext.CancellationToken);

                        await publishTask.UntilCompletedOrCanceled(sendContext.CancellationToken).ConfigureAwait(false);

                        sendContext.LogSent();

                        await _observers.PostSend(sendContext).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        sendContext.LogFaulted(ex);

                        await _observers.SendFault(sendContext, ex).ConfigureAwait(false);

                        throw;
                    }
                });
            });

            await _sessionContextSupervisor.Send(sessionPipe, cancellationToken).ConfigureAwait(false);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}
