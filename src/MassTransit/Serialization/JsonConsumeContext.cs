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
    using Transports;
    using Util;


    public class JsonConsumeContext :
        ConsumeContext
    {
        readonly JsonSerializer _deserializer;
        readonly MessageEnvelope _envelope;
        readonly JToken _messageToken;
        readonly IDictionary<Type, object> _messageTypes;
        readonly IPublishEndpoint _publishEndpoint;
        readonly ReceiveContext _receiveContext;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly string[] _supportedTypes;
        Guid? _correlationId;
        Uri _destinationAddress;
        Uri _faultAddress;
        Headers _headers;
        Guid? _messageId;
        Guid? _requestId;
        Uri _responseAddress;
        Uri _sourceAddress;

        public JsonConsumeContext(JsonSerializer deserializer, ISendEndpointProvider sendEndpointProvider, IPublishSendEndpointProvider publishEndpoint,
            ReceiveContext receiveContext, MessageEnvelope envelope)
        {
            _receiveContext = receiveContext;
            _envelope = envelope;
            _sendEndpointProvider = sendEndpointProvider;
            _deserializer = deserializer;
            _messageToken = GetMessageToken(envelope.Message);
            _supportedTypes = envelope.MessageType.ToArray();
            _messageTypes = new Dictionary<Type, object>();
            _publishEndpoint = new PublishEndpoint(receiveContext.InputAddress, publishEndpoint);
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

        public Guid? MessageId
        {
            get { return _messageId.HasValue ? _messageId : (_messageId = ConvertIdToGuid(_envelope.MessageId)); }
        }

        public Guid? RequestId
        {
            get { return _requestId.HasValue ? _requestId : (_requestId = ConvertIdToGuid(_envelope.RequestId)); }
        }

        public Guid? CorrelationId
        {
            get { return _correlationId.HasValue ? _correlationId : (_correlationId = ConvertIdToGuid(_envelope.CorrelationId)); }
        }

        public DateTime? ExpirationTime
        {
            get { return _envelope.ExpirationTime; }
        }

        public Uri SourceAddress
        {
            get { return _sourceAddress ?? (_sourceAddress = ConvertToUri(_envelope.SourceAddress)); }
        }

        public Uri DestinationAddress
        {
            get { return _destinationAddress ?? (_destinationAddress = ConvertToUri(_envelope.DestinationAddress)); }
        }

        public Uri ResponseAddress
        {
            get { return _responseAddress ?? (_responseAddress = ConvertToUri(_envelope.ResponseAddress)); }
        }

        public Uri FaultAddress
        {
            get { return _faultAddress ?? (_faultAddress = ConvertToUri(_envelope.FaultAddress)); }
        }

        public Headers Headers
        {
            get { return _headers ?? (_headers = new JsonMessageHeaders(_deserializer, _envelope.Headers)); }
        }

        public CancellationToken CancellationToken
        {
            get { return _receiveContext.CancellationToken; }
        }

        public ReceiveContext ReceiveContext
        {
            get { return _receiveContext; }
        }

        public Task CompleteTask
        {
            get { return _receiveContext.CompleteTask; }
        }

        public IEnumerable<string> SupportedMessageTypes
        {
            get { return _supportedTypes; }
        }

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
                await _publishEndpoint.Publish(message, new ResponsePipe<T>(this), CancellationToken).ConfigureAwait(false);
        }

        async Task ConsumeContext.RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
        {
            if (ResponseAddress != null)
            {
                ISendEndpoint endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await endpoint.Send(message, new ResponsePipe<T>(this, sendPipe), CancellationToken).ConfigureAwait(false);
            }
            else
                await _publishEndpoint.Publish(message, new ResponsePipe<T>(this, sendPipe), CancellationToken).ConfigureAwait(false);
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

            return new SendEndpointCapture(_receiveContext, sendEndpoint);
        }

        void ConsumeContext.NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            _receiveContext.NotifyConsumed(context, duration, consumerType);
        }

        public void NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            Task faultTask = GenerateFault(context, exception);

            _receiveContext.AddPendingTask(faultTask);

            Task receiveTask = _receiveContext.NotifyFaulted(context, duration, consumerType, exception);

            _receiveContext.AddPendingTask(receiveTask);
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Publish(message, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Publish(message, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Publish(message, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Publish(message, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Publish(message, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Publish(message, messageType, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Publish(message, messageType, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Publish<T>(values, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Publish(values, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            Task task = _publishEndpoint.Publish<T>(values, publishPipe, cancellationToken);
            _receiveContext.AddPendingTask(task);
            return task;
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishEndpoint.ConnectPublishObserver(observer);
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
                await _publishEndpoint.Publish(fault, faultPipe, CancellationToken);
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
            if (String.IsNullOrWhiteSpace(id))
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
            if (String.IsNullOrWhiteSpace(uri))
                return null;

            return new Uri(uri);
        }


        class SendEndpointCapture :
            ISendEndpoint
        {
            readonly ISendEndpoint _endpoint;
            readonly ReceiveContext _receiveContext;

            public SendEndpointCapture(ReceiveContext receiveContext, ISendEndpoint endpoint)
            {
                _receiveContext = receiveContext;
                _endpoint = endpoint;
            }

            public ConnectHandle ConnectSendObserver(ISendObserver observer)
            {
                return _endpoint.ConnectSendObserver(observer);
            }

            public Task Send<T>(T message, CancellationToken cancellationToken = new CancellationToken()) where T : class
            {
                Task task = _endpoint.Send(message, cancellationToken);
                _receiveContext.AddPendingTask(task);
                return task;
            }

            public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
            {
                Task task = _endpoint.Send(message, pipe, cancellationToken);
                _receiveContext.AddPendingTask(task);
                return task;
            }

            public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
            {
                Task task = _endpoint.Send(message, pipe, cancellationToken);
                _receiveContext.AddPendingTask(task);
                return task;
            }

            public Task Send(object message, CancellationToken cancellationToken = new CancellationToken())
            {
                Task task = _endpoint.Send(message, cancellationToken);
                _receiveContext.AddPendingTask(task);
                return task;
            }

            public Task Send(object message, Type messageType, CancellationToken cancellationToken = new CancellationToken())
            {
                Task task = _endpoint.Send(message, messageType, cancellationToken);
                _receiveContext.AddPendingTask(task);
                return task;
            }

            public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken())
            {
                Task task = _endpoint.Send(message, pipe, cancellationToken);
                _receiveContext.AddPendingTask(task);
                return task;
            }

            public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken())
            {
                Task task = _endpoint.Send(message, messageType, pipe, cancellationToken);
                _receiveContext.AddPendingTask(task);
                return task;
            }

            public Task Send<T>(object values, CancellationToken cancellationToken = new CancellationToken()) where T : class
            {
                Task task = _endpoint.Send<T>(values, cancellationToken);
                _receiveContext.AddPendingTask(task);
                return task;
            }

            public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
            {
                Task task = _endpoint.Send(values, pipe, cancellationToken);
                _receiveContext.AddPendingTask(task);
                return task;
            }

            public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
            {
                Task task = _endpoint.Send<T>(values, pipe, cancellationToken);
                _receiveContext.AddPendingTask(task);
                return task;
            }
        }
    }
}