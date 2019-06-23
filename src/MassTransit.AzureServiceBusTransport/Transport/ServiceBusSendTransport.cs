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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Scheduling;
    using Microsoft.ServiceBus.Messaging;
    using Transports;


    /// <summary>
    /// Send messages to an azure transport using the message sender.
    ///
    /// May be sensible to create a IBatchSendTransport that allows multiple
    /// messages to be sent as a single batch (perhaps using Tx support?)
    /// </summary>
    public class ServiceBusSendTransport :
        Supervisor,
        ISendTransport
    {
        readonly Uri _address;
        readonly SendObservable _observers;

        readonly IPipeContextSource<SendEndpointContext> _source;

        public ServiceBusSendTransport(IPipeContextSource<SendEndpointContext> source, Uri address)
        {
            _source = source;
            _address = address;
            _observers = new SendObservable();
        }

        Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            var clientPipe = new SendClientPipe<T>(message, pipe, cancellationToken, _observers);

            return _source.Send(clientPipe, cancellationToken);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        protected override Task StopSupervisor(StopSupervisorContext context)
        {
            LogContext.Debug?.Log("Stopping send transport: {Address}", _address);

            return base.StopSupervisor(context);
        }


        struct SendClientPipe<T> :
            IPipe<SendEndpointContext>
            where T : class
        {
            readonly T _message;
            readonly CancellationToken _cancellationToken;
            readonly IPipe<SendContext<T>> _pipe;
            readonly ISendObserver _observer;

            public SendClientPipe(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken, ISendObserver observer)
            {
                _message = message;
                _cancellationToken = cancellationToken;
                _pipe = pipe;
                _observer = observer;
            }

            public async Task Send(SendEndpointContext clientContext)
            {
                var context = new AzureServiceBusSendContext<T>(_message, _cancellationToken);

                try
                {
                    await _pipe.Send(context).ConfigureAwait(false);

                    CopyIncomingIdentifiersIfPresent(context);

                    if (IsCancelScheduledSend(context, out var sequenceNumber))
                    {
                        await CancelScheduledSend(clientContext, sequenceNumber).ConfigureAwait(false);

                        return;
                    }

                    if (context.ScheduledEnqueueTimeUtc.HasValue)
                    {
                        var scheduled = await ScheduleSend(clientContext, context).ConfigureAwait(false);
                        if (scheduled)
                            return;
                    }

                    await _observer.PreSend(context).ConfigureAwait(false);

                    var brokeredMessage = CreateBrokeredMessage(context);

                    await clientContext.Send(brokeredMessage).ConfigureAwait(false);

                    context.LogSent();

                    await _observer.PostSend(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await _observer.SendFault(context, ex).ConfigureAwait(false);

                    throw;
                }
            }

            public void Probe(ProbeContext context)
            {
            }

            static async Task<bool> ScheduleSend(SendEndpointContext clientContext, AzureServiceBusSendContext<T> context)
            {
                var now = DateTime.UtcNow;

                var enqueueTimeUtc = context.ScheduledEnqueueTimeUtc.Value;
                if (enqueueTimeUtc < now)
                {
                    LogContext.Debug?.Log("The scheduled time was in the past, sending: {ScheduledTime}", context.ScheduledEnqueueTimeUtc);

                    return false;
                }

                try
                {
                    var brokeredMessage = CreateBrokeredMessage(context);

                    var sequenceNumber = await clientContext.ScheduleSend(brokeredMessage, enqueueTimeUtc).ConfigureAwait(false);

                    context.SetScheduledMessageId(sequenceNumber);

                    context.LogScheduled(enqueueTimeUtc);

                    return true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    LogContext.Debug?.Log("The scheduled time was rejected by the server, sending: {MessageId}", context.MessageId);

                    return false;
                }
            }

            static async Task CancelScheduledSend(SendEndpointContext clientContext, long sequenceNumber)
            {
                try
                {
                    await clientContext.CancelScheduledSend(sequenceNumber).ConfigureAwait(false);

                    LogContext.Debug?.Log("Canceled scheduled message {SequenceNumber} {EntityPath}", sequenceNumber, clientContext.EntityPath);
                }
                catch (MessageNotFoundException exception)
                {
                    LogContext.Warning?.Log(exception, "The scheduled message was not found: {SequenceNumber} {EntityPath}", sequenceNumber,
                        clientContext.EntityPath);
                }
            }

            bool IsCancelScheduledSend(AzureServiceBusSendContext<T> context, out long sequenceNumber)
            {
                if (_message is CancelScheduledMessage cancelScheduledMessage)
                {
                    if (context.TryGetScheduledMessageId(out sequenceNumber)
                        || context.TryGetSequenceNumber(cancelScheduledMessage.TokenId, out sequenceNumber))
                        return true;
                }

                sequenceNumber = default;
                return false;
            }

            static BrokeredMessage CreateBrokeredMessage(AzureServiceBusSendContext<T> context)
            {
                var brokeredMessage = new BrokeredMessage(context.GetBodyStream())
                {
                    ContentType = context.ContentType.MediaType,
                    ForcePersistence = context.Durable
                };

                brokeredMessage.Properties.SetTextHeaders(context.Headers, (_, text) => text);

                if (context.TimeToLive.HasValue)
                    brokeredMessage.TimeToLive = context.TimeToLive.Value;

                if (context.MessageId.HasValue)
                    brokeredMessage.MessageId = context.MessageId.Value.ToString("N");

                if (context.CorrelationId.HasValue)
                    brokeredMessage.CorrelationId = context.CorrelationId.Value.ToString("N");

                if (context.PartitionKey != null)
                    brokeredMessage.PartitionKey = context.PartitionKey;

                if (!string.IsNullOrWhiteSpace(context.SessionId))
                {
                    brokeredMessage.SessionId = context.SessionId;

                    if (context.ReplyToSessionId == null)
                        brokeredMessage.ReplyToSessionId = context.SessionId;
                }

                if (context.ReplyToSessionId != null)
                    brokeredMessage.ReplyToSessionId = context.ReplyToSessionId;

                return brokeredMessage;
            }

            static void CopyIncomingIdentifiersIfPresent(AzureServiceBusSendContext<T> sendContext)
            {
                if (sendContext.TryGetPayload<ConsumeContext>(out var consumeContext)
                    && consumeContext.TryGetPayload<BrokeredMessageContext>(out var brokeredMessageContext))
                {
                    if (sendContext.SessionId == null)
                    {
                        if (brokeredMessageContext.ReplyToSessionId != null)
                            sendContext.SessionId = brokeredMessageContext.ReplyToSessionId;
                        else if (brokeredMessageContext.SessionId != null)
                            sendContext.SessionId = brokeredMessageContext.SessionId;
                    }

                    if (sendContext.PartitionKey == null && brokeredMessageContext.PartitionKey != null)
                        sendContext.PartitionKey = brokeredMessageContext.PartitionKey;
                }
            }
        }
    }
}
