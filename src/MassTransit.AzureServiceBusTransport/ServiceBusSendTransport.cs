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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Logging;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;
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

        readonly SendObservable _observers;
        readonly MessageSender _sender;
        ITaskParticipant _participant;

        public ServiceBusSendTransport(MessageSender sender, ITaskSupervisor supervisor)
        {
            _observers = new SendObservable();
            _sender = sender;

            _participant = supervisor.CreateParticipant($"{TypeMetadataCache<ServiceBusSendTransport>.ShortName} - {sender.Path}", StopSender);
        }

        async Task StopSender()
        {
            try
            {
                await _sender.CloseAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _log.Error($"Failed to close message sender: {_sender.Path}", exception);
                throw;
            }
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            var context = new ServiceBusSendContextImpl<T>(message, cancelSend);

            try
            {
                await pipe.Send(context).ConfigureAwait(false);

                using (var messageBodyStream = context.GetBodyStream())
                {
                    using (var brokeredMessage = new BrokeredMessage(messageBodyStream))
                    {
                        brokeredMessage.ContentType = context.ContentType.MediaType;
                        brokeredMessage.ForcePersistence = context.Durable;

                        if (context.TimeToLive.HasValue)
                            brokeredMessage.TimeToLive = context.TimeToLive.Value;

                        if (context.MessageId.HasValue)
                            brokeredMessage.MessageId = context.MessageId.Value.ToString("N");

                        if (context.CorrelationId.HasValue)
                            brokeredMessage.CorrelationId = context.CorrelationId.Value.ToString("N");

                        if (context.ScheduledEnqueueTimeUtc.HasValue)
                            brokeredMessage.ScheduledEnqueueTimeUtc = context.ScheduledEnqueueTimeUtc.Value;

                        if (context.PartitionKey != null)
                            brokeredMessage.PartitionKey = context.PartitionKey;

                        if (context.SessionId != null)
                        {
                            brokeredMessage.SessionId = context.SessionId;

                            if (context.ReplyToSessionId == null)
                                brokeredMessage.ReplyToSessionId = context.SessionId;
                        }

                        if (context.ReplyToSessionId != null)
                            brokeredMessage.ReplyToSessionId = context.ReplyToSessionId;

                        await _observers.PreSend(context).ConfigureAwait(false);

                        await _sender.SendAsync(brokeredMessage).ConfigureAwait(false);

                        _log.DebugFormat("SEND {0} ({1})", brokeredMessage.MessageId, _sender.Path);

                        await _observers.PostSend(context).ConfigureAwait(false);
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
            BrokeredMessageContext messageContext;
            if (context.TryGetPayload(out messageContext))
            {
                using (var messageBodyStream = context.GetBody())
                {
                    using (var brokeredMessage = new BrokeredMessage(messageBodyStream))
                    {
                        brokeredMessage.ContentType = context.ContentType.MediaType;
                        brokeredMessage.ForcePersistence = messageContext.ForcePersistence;
                        brokeredMessage.TimeToLive = messageContext.TimeToLive;
                        brokeredMessage.CorrelationId = messageContext.CorrelationId;
                        brokeredMessage.MessageId = messageContext.MessageId;
                        brokeredMessage.Label = messageContext.Label;
                        brokeredMessage.PartitionKey = messageContext.PartitionKey;
                        brokeredMessage.ReplyTo = messageContext.ReplyTo;
                        brokeredMessage.ReplyToSessionId = messageContext.ReplyToSessionId;
                        brokeredMessage.SessionId = messageContext.SessionId;

                        await _sender.SendAsync(brokeredMessage).ConfigureAwait(false);

                        _log.DebugFormat("MOVE {0} ({1} to {2})", brokeredMessage.MessageId, context.InputAddress, _sender.Path);
                    }
                }
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
                await _sender.CloseAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.Error($"The message sender could not be closed: {_sender.Path}", ex);
            }
        }
    }
}