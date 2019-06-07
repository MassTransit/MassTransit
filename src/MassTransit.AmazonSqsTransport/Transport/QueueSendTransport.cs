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
namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using MassTransit.Pipeline.Observables;
    using Transports;


    public class QueueSendTransport :
        Supervisor,
        ISendTransport
    {
        readonly string _entityName;
        readonly IFilter<ClientContext> _filter;
        readonly IPipeContextSource<ClientContext> _clientSource;
        readonly SendObservable _observers;

        public QueueSendTransport(IPipeContextSource<ClientContext> clientSource, IFilter<ClientContext> preSendFilter, string entityName)
        {
            _clientSource = clientSource;
            _filter = preSendFilter;
            _entityName = entityName;

            _observers = new SendObservable();
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            if (IsStopped)
                throw new TransportUnavailableException($"The send transport is stopped: {_entityName}");

            IPipe<ClientContext> modelPipe = Pipe.New<ClientContext>(p =>
            {
                p.UseFilter(_filter);

                p.UseExecuteAsync(async clientContext =>
                {
                    var sendContext = new TransportAmazonSqsSendContext<T>(message, cancellationToken);
                    try
                    {
                        await pipe.Send(sendContext).ConfigureAwait(false);

                        var transportMessage = clientContext.CreateSendRequest(_entityName, sendContext.Body);

                        transportMessage.MessageAttributes.Set(sendContext.Headers);

                        if (!string.IsNullOrEmpty(sendContext.DeduplicationId))
                            transportMessage.MessageDeduplicationId = sendContext.DeduplicationId;

                        if (!string.IsNullOrEmpty(sendContext.GroupId))
                            transportMessage.MessageGroupId = sendContext.GroupId;

                        if (sendContext.DelaySeconds.HasValue)
                            transportMessage.DelaySeconds = sendContext.DelaySeconds.Value;

                        transportMessage.MessageAttributes.Set("Content-Type", sendContext.ContentType.MediaType);
                        transportMessage.MessageAttributes.Set(nameof(sendContext.MessageId), sendContext.MessageId);
                        transportMessage.MessageAttributes.Set(nameof(sendContext.CorrelationId), sendContext.CorrelationId);
                        transportMessage.MessageAttributes.Set(nameof(sendContext.TimeToLive), sendContext.TimeToLive);

                        await _observers.PreSend(sendContext).ConfigureAwait(false);

                        await clientContext.SendMessage(transportMessage, sendContext.CancellationToken).ConfigureAwait(false);

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

            await _clientSource.Send(modelPipe, cancellationToken).ConfigureAwait(false);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}
