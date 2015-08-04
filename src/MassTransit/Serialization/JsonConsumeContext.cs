// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Events;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Pipeline;
    using Pipeline.Pipes;
    using Util;


    public class JsonConsumeContext :
        ConsumeContext
    {
        readonly JsonSerializer _deserializer;
        readonly MessageEnvelope _envelope;
        readonly JToken _messageToken;
        readonly IDictionary<Type, object> _messageTypes;
        readonly ReceiveContext _receiveContext;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly IPublishEndpointProvider _publishEndpointProvider;
        readonly string[] _supportedTypes;
        readonly Lazy<IPublishEndpoint> _publishEndpoint;
        Guid? _conversationId;
        Guid? _correlationId;
        Uri _destinationAddress;
        Uri _faultAddress;
        Headers _headers;
        Guid? _initiatorId;
        Guid? _messageId;
        Guid? _requestId;
        Uri _responseAddress;
        Uri _sourceAddress;

        public JsonConsumeContext(JsonSerializer deserializer, ISendEndpointProvider sendEndpointProvider, IPublishEndpointProvider publishEndpointProvider,
            ReceiveContext receiveContext, MessageEnvelope envelope)
        {
            _receiveContext = receiveContext;
            _envelope = envelope;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpointProvider = publishEndpointProvider;
            _deserializer = deserializer;
            _messageToken = GetMessageToken(envelope.Message);
            _supportedTypes = envelope.MessageType.ToArray();
            _messageTypes = new Dictionary<Type, object>();

            _publishEndpoint = new Lazy<IPublishEndpoint>(() => _publishEndpointProvider.CreatePublishEndpoint(_receiveContext.InputAddress, CorrelationId, ConversationId));
        }

        public bool HasPayloadType(Type contextType)
        {
            return _receiveContext.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload payload)
            where TPayload : class
        {
            return _receiveContext.TryGetPayload(out payload);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            return _receiveContext.GetOrAddPayload(payloadFactory);
        }

        public Guid? MessageId => _messageId.HasValue ? _messageId : (_messageId = ConvertIdToGuid(_envelope.MessageId));
        public Guid? RequestId => _requestId.HasValue ? _requestId : (_requestId = ConvertIdToGuid(_envelope.RequestId));
        public Guid? CorrelationId => _correlationId.HasValue ? _correlationId : (_correlationId = ConvertIdToGuid(_envelope.CorrelationId));
        public Guid? ConversationId => _conversationId.HasValue ? _conversationId : (_conversationId = ConvertIdToGuid(_envelope.ConversationId));
        public Guid? InitiatorId => _initiatorId.HasValue ? _initiatorId : (_initiatorId = ConvertIdToGuid(_envelope.InitiatorId));
        public DateTime? ExpirationTime => _envelope.ExpirationTime;
        public Uri SourceAddress => _sourceAddress ?? (_sourceAddress = ConvertToUri(_envelope.SourceAddress));
        public Uri DestinationAddress => _destinationAddress ?? (_destinationAddress = ConvertToUri(_envelope.DestinationAddress));
        public Uri ResponseAddress => _responseAddress ?? (_responseAddress = ConvertToUri(_envelope.ResponseAddress));
        public Uri FaultAddress => _faultAddress ?? (_faultAddress = ConvertToUri(_envelope.FaultAddress));
        public Headers Headers => _headers ?? (_headers = new JsonMessageHeaders(_deserializer, _envelope.Headers));
        public CancellationToken CancellationToken => _receiveContext.CancellationToken;
        public ReceiveContext ReceiveContext => _receiveContext;
        public Task CompleteTask => _receiveContext.CompleteTask;
        public IEnumerable<string> SupportedMessageTypes => _supportedTypes;

        public bool HasMessageType(Type messageType)
        {
            lock (_messageTypes)
            {
                object existing;
                if (_messageTypes.TryGetValue(messageType, out existing))
                    return existing != null;
            }

            string typeUrn = new MessageUrn(messageType).ToString();

            return _supportedTypes.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase));
        }

        public bool TryGetMessage<T>(out ConsumeContext<T> message)
            where T : class
        {
            lock (_messageTypes)
            {
                object existing;
                if (_messageTypes.TryGetValue(typeof(T), out existing))
                {
                    message = existing as ConsumeContext<T>;
                    return message != null;
                }

                if (typeof(T) == typeof(JToken))
                {
                    _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, _messageToken as T);
                    return true;
                }

                string typeUrn = new MessageUrn(typeof(T)).ToString();

                if (_supportedTypes.Any(typeUrn.Equals))
                {
                    object obj;
                    Type deserializeType = typeof(T);
                    if (deserializeType.IsInterface && TypeMetadataCache.IsValidMessageType(deserializeType))
                        deserializeType = TypeMetadataCache.GetImplementationType(deserializeType);

                    using (JsonReader jsonReader = _messageToken.CreateReader())
                    {
                        obj = _deserializer.Deserialize(jsonReader, deserializeType);
                    }

                    _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, (T)obj);
                    return true;
                }

                _messageTypes[typeof(T)] = message = null;
                return false;
            }
        }

        public async Task RespondAsync<T>(T message)
            where T : class
        {
            if (ResponseAddress != null)
            {
                ISendEndpoint endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await endpoint.Send(message, new ResponsePipe<T>(this), CancellationToken).ConfigureAwait(false);
            }
            else
                await _publishEndpoint.Value.Publish(message, new ResponsePipe<T>(this), CancellationToken).ConfigureAwait(false);
        }

        async Task ConsumeContext.RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
        {
            if (ResponseAddress != null)
            {
                ISendEndpoint endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await endpoint.Send(message, new ResponsePipe<T>(this, sendPipe), CancellationToken).ConfigureAwait(false);
            }
            else
                await _publishEndpoint.Value.Publish(message, new ResponsePipe<T>(this, sendPipe), CancellationToken).ConfigureAwait(false);
        }

        public void Respond<T>(T message)
            where T : class
        {
            Task task = RespondAsync(message);

            _receiveContext.AddPendingTask(task);
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(address).ConfigureAwait(false);

            return new ConsumeSendEndpoint(sendEndpoint, this);
        }

        Task ConsumeContext.NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            Task receiveTask = _receiveContext.NotifyConsumed(context, duration, consumerType);

            _receiveContext.AddPendingTask(receiveTask);

            return TaskUtil.Completed;
        }

        public Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            Task faultTask = GenerateFault(context, exception);

            _receiveContext.AddPendingTask(faultTask);

            Task receiveTask = _receiveContext.NotifyFaulted(context, duration, consumerType, exception);

            _receiveContext.AddPendingTask(receiveTask);

            return TaskUtil.Completed;
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Value.Publish(message, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Value.Publish(message, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Value.Publish(message, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Value.Publish(message, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Value.Publish(message, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Value.Publish(message, messageType, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Value.Publish(message, messageType, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Value.Publish<T>(values, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Value.Publish(values, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Value.Publish<T>(values, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishEndpoint.Value.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendEndpointProvider.ConnectSendObserver(observer);
        }

        async Task GenerateFault<T>(ConsumeContext<T> context, Exception exception)
            where T : class
        {
            Fault<T> fault = new FaultEvent<T>(context.Message, HostMetadataCache.Host, exception);

            IPipe<SendContext<Fault<T>>> faultPipe = Pipe.Execute<SendContext<Fault<T>>>(x =>
            {
                x.SourceAddress = ReceiveContext.InputAddress;
                x.CorrelationId = CorrelationId;
                x.RequestId = RequestId;

                foreach (var header in Headers.GetAll())
                    x.Headers.Set(header.Key, header.Value);
            });

            if (FaultAddress != null)
            {
                ISendEndpoint endpoint = await GetSendEndpoint(FaultAddress);

                await endpoint.Send(fault, faultPipe, CancellationToken);
            }
            else if (ResponseAddress != null)
            {
                ISendEndpoint endpoint = await GetSendEndpoint(ResponseAddress);

                await endpoint.Send(fault, faultPipe, CancellationToken);
            }
            else
                await _publishEndpoint.Value.Publish(fault, faultPipe, CancellationToken);
        }

        static JToken GetMessageToken(object message)
        {
            var messageToken = message as JToken;
            if (messageToken == null)
                return new JObject();

            if (messageToken.Type == JTokenType.Null)
                return new JObject();

            return messageToken;
        }

        /// <summary>
        ///     Converts a string identifier to a Guid, if it's actually a Guid. Can throw a FormatException
        ///     if things are not right
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static Guid? ConvertIdToGuid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return default(Guid?);

            Guid messageId;
            if (Guid.TryParse(id, out messageId))
                return messageId;

            throw new FormatException("The Id was not a Guid: " + id);
        }

        /// <summary>
        ///     Convert the string to a Uri, or return null if it's empty
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        static Uri ConvertToUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                return null;

            return new Uri(uri);
        }
    }
}