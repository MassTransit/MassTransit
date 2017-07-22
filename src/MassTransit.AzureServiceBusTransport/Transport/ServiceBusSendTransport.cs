// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Logging;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Scheduling;
    using Microsoft.ServiceBus.Messaging;
    using Serialization;
    using Transports;
    using Util;


    /// <summary>
    /// Send messages to an azure transport using the message sender.
    /// 
    /// May be sensible to create a IBatchSendTransport that allows multiple
    /// messages to be sent as a single batch (perhaps using Tx support?)
    /// </summary>
    public class ServiceBusSendTransport :
        ISendTransport
    {
        static readonly ILog _log = Logger.Get<ServiceBusSendTransport>();
        readonly ISendClient _client;

        readonly SendObservable _observers;
        ITaskParticipant _participant;

        public ServiceBusSendTransport(ISendClient client, ITaskSupervisor supervisor)
        {
            _client = client;
            _observers = new SendObservable();

            _participant = supervisor.CreateParticipant($"{TypeMetadataCache<ServiceBusSendTransport>.ShortName} - {client.Path}", StopSender);
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            var context = new AzureServiceBusSendContext<T>(message, cancelSend);

            try
            {
                await pipe.Send(context).ConfigureAwait(false);

                var cancelScheduledMessage = message as CancelScheduledMessage;
                if (cancelScheduledMessage != null)
                {
                    try
                    {
                        long sequenceNumber;
                        if (context.TryGetScheduledMessageId(out sequenceNumber))
                        {
                            await _client.CancelScheduledSend(sequenceNumber).ConfigureAwait(false);
                        }
                        else
                        {
                            sequenceNumber = context.GetSequenceNumber(cancelScheduledMessage.TokenId);

                            await _client.CancelScheduledSend(sequenceNumber).ConfigureAwait(false);
                        }

                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Canceled Scheduled: {0} {1}", sequenceNumber, _client.Path);

                    }
                    catch (MessageNotFoundException exception)
                    {
                        if(_log.IsDebugEnabled)
                            _log.DebugFormat("The scheduled message was not found: {0}", exception.Detail.Message);
                    }
                }
                else
                {
                    await _observers.PreSend(context).ConfigureAwait(false);

                    using (var messageBodyStream = context.GetBodyStream())
                    {
                        using (var brokeredMessage = new BrokeredMessage(messageBodyStream))
                        {
                            brokeredMessage.ContentType = context.ContentType.MediaType;
                            brokeredMessage.ForcePersistence = context.Durable;

                            KeyValuePair<string, object>[] headers = context.Headers.GetAll()
                                .Where(x => x.Value != null && (x.Value is string || x.Value.GetType().IsValueType))
                                .ToArray();

                            foreach (KeyValuePair<string, object> header in headers)
                            {
                                if (brokeredMessage.Properties.ContainsKey(header.Key))
                                    continue;

                                brokeredMessage.Properties.Add(header.Key, header.Value);
                            }

                            if (context.TimeToLive.HasValue)
                                brokeredMessage.TimeToLive = context.TimeToLive.Value;

                            if (context.MessageId.HasValue)
                                brokeredMessage.MessageId = context.MessageId.Value.ToString("N");

                            if (context.CorrelationId.HasValue)
                                brokeredMessage.CorrelationId = context.CorrelationId.Value.ToString("N");

                            if (context.PartitionKey != null)
                                brokeredMessage.PartitionKey = context.PartitionKey;

                            var sessionId = string.IsNullOrWhiteSpace(context.SessionId) ? context.ConversationId?.ToString("N") : context.SessionId;
                            if (!string.IsNullOrWhiteSpace(sessionId))
                            {
                                brokeredMessage.SessionId = sessionId;

                                if (context.ReplyToSessionId == null)
                                    brokeredMessage.ReplyToSessionId = sessionId;
                            }

                            if (context.ReplyToSessionId != null)
                                brokeredMessage.ReplyToSessionId = context.ReplyToSessionId;

                            if (context.ScheduledEnqueueTimeUtc.HasValue)
                            {
                                var enqueueTimeUtc = context.ScheduledEnqueueTimeUtc.Value;

                                var sequenceNumber = await _client.ScheduleSend(brokeredMessage, enqueueTimeUtc).ConfigureAwait(false);

                                context.SetScheduledMessageId(sequenceNumber);

                                context.LogScheduled(enqueueTimeUtc);
                            }
                            else
                            {
                                await _client.Send(brokeredMessage).ConfigureAwait(false);

                                context.LogSent();

                                await _observers.PostSend(context).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _observers.SendFault(context, ex).ConfigureAwait(false);

                throw;
            }
        }

        async Task ISendTransport.Move(ReceiveContext context, IPipe<SendContext> pipe)
        {
            try
            {
                using (var moveContext = new ServiceBusMoveContext(context))
                {
                    await pipe.Send(moveContext).ConfigureAwait(false);

                    await _client.Send(moveContext.BrokeredMessage).ConfigureAwait(false);

                    _log.DebugFormat("MOVE {0} ({1} to {2})", moveContext.BrokeredMessage.MessageId, context.InputAddress, _client.Path);
                }
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.Error("Move To Error Queue Fault: " + _client.Path, ex);

                throw;
            }
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        public async Task Close()
        {
            try
            {
                await _client.Close().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.Error($"The message sender could not be closed: {_client.Path}", ex);
            }
        }

        async Task StopSender()
        {
            try
            {
                await _client.Close().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _log.Error($"Failed to close message sender: {_client.Path}", exception);
                throw;
            }
        }


        class ServiceBusMoveContext :
            ServiceBusSendContext,
            IDisposable
        {
            readonly BrokeredMessage _brokeredMessage;
            readonly ReceiveContext _context;

            readonly Stream _messageBodyStream;
            IMessageSerializer _serializer;

            public ServiceBusMoveContext(ReceiveContext context)
            {
                _context = context;
                _serializer = new CopyBodySerializer(context);

                BrokeredMessageContext messageContext;
                if (!context.TryGetPayload(out messageContext))
                    throw new ArgumentException("The context must be a service bus receive context", nameof(context));

                _messageBodyStream = context.GetBody();
                _brokeredMessage = new BrokeredMessage(_messageBodyStream)
                {
                    ContentType = context.ContentType.MediaType,
                    ForcePersistence = messageContext.ForcePersistence,
                    TimeToLive = messageContext.TimeToLive,
                    CorrelationId = messageContext.CorrelationId,
                    MessageId = messageContext.MessageId,
                    Label = messageContext.Label,
                    PartitionKey = messageContext.PartitionKey,
                    ReplyTo = messageContext.ReplyTo,
                    ReplyToSessionId = messageContext.ReplyToSessionId,
                    SessionId = messageContext.SessionId
                };

                Headers = new DictionarySendHeaders(_brokeredMessage.Properties);

                foreach (KeyValuePair<string, object> property in messageContext.Properties)
                {
                    _brokeredMessage.Properties[property.Key] = property.Value;
                }
            }

            public BrokeredMessage BrokeredMessage => _brokeredMessage;

            public void Dispose()
            {
                _brokeredMessage.Dispose();
                _messageBodyStream.Dispose();
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

            public DateTime? ScheduledEnqueueTimeUtc { get; set; }
            public string PartitionKey { get; set; }

            public string SessionId { get; set; }
            public string ReplyToSessionId { get; set; }
        }
    }
}