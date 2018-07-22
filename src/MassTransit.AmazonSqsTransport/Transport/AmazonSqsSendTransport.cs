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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService.Model;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using MassTransit.Pipeline.Observables;
    using Transports;

    public class AmazonSqsSendTransport :
        Supervisor,
        ISendTransport
    {
        readonly string _entityName;
        readonly IFilter<ModelContext> _filter;
        readonly IPipeContextSource<ModelContext> _modelAgent;
        readonly SendObservable _observers;

        public AmazonSqsSendTransport(IAgent<ModelContext> modelAgent, IFilter<ModelContext> preSendFilter, string entityName)
        {
            _modelAgent = modelAgent;
            _filter = preSendFilter;
            _entityName = entityName;

            _observers = new SendObservable();
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            if (IsStopped)
                throw new TransportUnavailableException($"The send transport is stopped: {_entityName}");

            IPipe<ModelContext> modelPipe = Pipe.New<ModelContext>(p =>
            {
                p.UseFilter(_filter);

                p.UseExecuteAsync(async modelContext =>
                {
                    var topicArn = await modelContext.GetTopic(_entityName).ConfigureAwait(false);

                    var sendContext = new TransportAmazonSqsSendContext<T>(message, cancellationToken);
                    try
                    {
                        await pipe.Send(sendContext).ConfigureAwait(false);

                        var transportMessage = modelContext.CreateTransportMessage(topicArn, sendContext.Body);

                        KeyValuePair<string, object>[] headers = sendContext.Headers.GetAll()
                            .Where(x => x.Value != null && (x.Value is string || x.Value.GetType().GetTypeInfo().IsValueType))
                            .ToArray();

                        foreach (KeyValuePair<string, object> header in headers)
                        {
                            if (transportMessage.MessageAttributes.ContainsKey(header.Key))
                                continue;

                            transportMessage.MessageAttributes[header.Key].StringValue = header.Value.ToString();
                        }

                        transportMessage.MessageAttributes.Add("Content-Type", new MessageAttributeValue
                        {
                            DataType = "String",
                            StringValue = sendContext.ContentType.MediaType
                        });

                        if (sendContext.MessageId.HasValue)
                            transportMessage.MessageAttributes.Add("MessageId", new MessageAttributeValue
                            {
                                DataType = "String",
                                StringValue = sendContext.MessageId.ToString()
                            });

                        if (sendContext.CorrelationId.HasValue)
                            transportMessage.MessageAttributes.Add("CorrelationId", new MessageAttributeValue
                            {
                                DataType = "String",
                                StringValue = sendContext.CorrelationId.ToString()
                            });

                        if (sendContext.TimeToLive.HasValue)
                            transportMessage.MessageAttributes.Add("TimeToLive", new MessageAttributeValue
                            {
                                DataType = "Number",
                                StringValue = sendContext.TimeToLive.Value.Milliseconds.ToString()
                            });

                        await _observers.PreSend(sendContext).ConfigureAwait(false);

                        var publishTask = Task.Run(() => modelContext.Publish(transportMessage, sendContext.CancellationToken), sendContext.CancellationToken);

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

            await _modelAgent.Send(modelPipe, cancellationToken).ConfigureAwait(false);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}
