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
    using System.Runtime.Remoting.Messaging;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Events;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Pipeline;
    using Pipeline.Pipes;
    using Util;


    /// <summary>
    /// A static consume context from the Binary serializer
    /// </summary>
    public class StaticConsumeContext :
        ConsumeContext
    {
        readonly Header[] _binaryHeaders;
        readonly JsonSerializer _deserializer;
        readonly object _message;
        readonly IDictionary<Type, object> _messageTypes;
        readonly IList<Task> _pendingTasks;
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

        public StaticConsumeContext(JsonSerializer deserializer, ISendEndpointProvider sendEndpointProvider,
            IPublishEndpoint publishEndpoint,
            ReceiveContext receiveContext, object message, Header[] headers)
        {
            _deserializer = deserializer;
            _receiveContext = receiveContext;
            _sendEndpointProvider = sendEndpointProvider;
            _messageTypes = new Dictionary<Type, object>();
            _publishEndpoint = publishEndpoint;
            _pendingTasks = new List<Task>();
            _message = message;
            _binaryHeaders = headers;

            _supportedTypes = new[] {GetHeaderString(BinaryMessageSerializer.MessageTypeKey)};
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
            get { return _messageId.HasValue ? _messageId : (_messageId = GetHeaderGuid(BinaryMessageSerializer.MessageIdKey)); }
        }

        public Guid? RequestId
        {
            get { return _requestId.HasValue ? _requestId : (_requestId = GetHeaderGuid(BinaryMessageSerializer.RequestIdKey)); }
        }

        public Guid? CorrelationId
        {
            get { return _correlationId.HasValue ? _correlationId : (_correlationId = GetHeaderGuid(BinaryMessageSerializer.CorrelationIdKey)); }
        }

        public DateTime? ExpirationTime
        {
            get { return GetHeaderDateTime(BinaryMessageSerializer.ExpirationTimeKey); }
        }

        public Uri SourceAddress
        {
            get { return _sourceAddress ?? (_sourceAddress = GetHeaderUri(BinaryMessageSerializer.SourceAddressKey)); }
        }

        public Uri DestinationAddress
        {
            get { return _destinationAddress ?? (_destinationAddress = GetHeaderUri(BinaryMessageSerializer.DestinationAddressKey)); }
        }

        public Uri ResponseAddress
        {
            get { return _responseAddress ?? (_responseAddress = GetHeaderUri(BinaryMessageSerializer.ResponseAddressKey)); }
        }

        public Uri FaultAddress
        {
            get { return _faultAddress ?? (_faultAddress = GetHeaderUri(BinaryMessageSerializer.FaultAddressKey)); }
        }

        public Headers Headers
        {
            get { return _headers ?? (_headers = new StaticHeaders(_deserializer, _binaryHeaders)); }
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
            get { return Task.WhenAll(_pendingTasks); }
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

                string typeUrn = new MessageUrn(typeof(T)).ToString();

                if (_supportedTypes.Any(typeUrn.Equals))
                {
                    if (_message is T)
                    {
                        _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, (T)_message);
                        return true;
                    }

                    message = null;
                    return false;
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
            {
                await _publishEndpoint.Publish(message, new ResponsePipe<T>(this), CancellationToken).ConfigureAwait(false);
            }
        }

        async Task ConsumeContext.RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
        {
            if (ResponseAddress != null)
            {
                ISendEndpoint endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await endpoint.Send(message, new ResponsePipe<T>(this, sendPipe), CancellationToken).ConfigureAwait(false);
            }
            else
            {
                await _publishEndpoint.Publish(message, new ResponsePipe<T>(this, sendPipe), CancellationToken).ConfigureAwait(false);
            }
        }

        public void Respond<T>(T message)
            where T : class
        {
            Task task = RespondAsync(message);

            _pendingTasks.Add(task);
        }

        public void RetryLater()
        {
            throw new NotImplementedException();
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _sendEndpointProvider.GetSendEndpoint(address);
        }

        void ConsumeContext.NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            _receiveContext.NotifyConsumed(context, duration, consumerType);
        }

        public void NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            Task faultTask = GenerateFault(context, exception);

            _pendingTasks.Add(faultTask);

            Task receiveTask = _receiveContext.NotifyFaulted(context, duration, consumerType, exception);

            _pendingTasks.Add(receiveTask);
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish<T>(values, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish<T>(values, publishPipe, cancellationToken);
        }

        async Task GenerateFault<T>(T message, Exception exception)
            where T : class
        {
            Fault<T> fault = new FaultEvent<T>(message, HostMetadataCache.Host, exception);

            IPipe<SendContext<Fault<T>>> faultPipe = Pipe.New<SendContext<Fault<T>>>(x => x.Execute(v =>
            {
                v.SourceAddress = ReceiveContext.InputAddress;
                v.CorrelationId = CorrelationId;
                v.RequestId = RequestId;

                foreach (var header in Headers.GetAll())
                    v.Headers.Set(header.Item1, header.Item2);
            }));

            if (ResponseAddress != null)
            {
                ISendEndpoint endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await endpoint.Send(fault, faultPipe, CancellationToken).ConfigureAwait(false);
            }
            else
                await _publishEndpoint.Publish(fault, faultPipe, CancellationToken).ConfigureAwait(false);
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

        string GetHeaderString(string headerName)
        {
            object header = GetHeader(headerName);
            if (header == null)
                return null;

            var s = header as string;
            if (s != null)
                return s;

            var uri = header as Uri;
            if (uri != null)
                return uri.ToString();

            return header.ToString();
        }

        Uri GetHeaderUri(string headerName)
        {
            try
            {
                object header = GetHeader(headerName);
                if (header == null)
                    return null;

                var uri = header as Uri;
                if (uri != null)
                    return uri;

                var s = header as string;
                if (s != null)
                    return new Uri(s);
            }
            catch (UriFormatException)
            {
            }

            return null;
        }

        Guid? GetHeaderGuid(string headerName)
        {
            try
            {
                object header = GetHeader(headerName);
                if (header == null)
                    return default(Guid?);

                if (header is Guid)
                    return (Guid)header;

                var s = header as string;
                if (s != null)
                    return new Guid(s);
            }
            catch (FormatException)
            {
            }

            return default(Guid?);
        }

        DateTime? GetHeaderDateTime(string headerName)
        {
            try
            {
                object header = GetHeader(headerName);
                if (header == null)
                    return default(DateTime?);

                if (header is DateTime)
                    return (DateTime)header;

                var s = header as string;
                if (s != null)
                    return DateTime.Parse(s);
            }
            catch (FormatException)
            {
            }

            return default(DateTime?);
        }

        object GetHeader(string headerName)
        {
            return _binaryHeaders.Where(x => x.Name == headerName).Select(x => x.Value).FirstOrDefault();
        }

        public ConnectHandle Connect(IPublishObserver observer)
        {
            return _publishEndpoint.Connect(observer);
        }
    }
}