// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Serialization;
    using Serialization.Custom;


    public class JsonConsumeContext :
        ConsumeContext
    {
        readonly JsonSerializer _deserializer;
        readonly MessageEnvelope _envelope;
        readonly JToken _messageToken;
        readonly IDictionary<Type, object> _messageTypes;
        readonly ReceiveContext _receiveContext;
        readonly string[] _supportedTypes;
        Guid? _correlationId;


        Uri _destinationAddress;
        Uri _faultAddress;
        ContextHeaders _headers;
        Guid? _messageId;
        Guid? _requestId;
        Uri _responseAddress;
        Uri _sourceAddress;

        public JsonConsumeContext(JsonSerializer deserializer, ReceiveContext receiveContext, MessageEnvelope envelope)
        {
            _receiveContext = receiveContext;
            _envelope = envelope;
            _deserializer = deserializer;
            _messageToken = GetMessageToken(envelope.Message);
            _supportedTypes = envelope.MessageType.ToArray();
            _messageTypes = new Dictionary<Type, object>();
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

        public ContextHeaders ContextHeaders
        {
            get { return _headers ?? (_headers = new JsonMessageContextHeaders(_deserializer, _envelope.Headers)); }
        }

        public CancellationToken CancellationToken
        {
            get { return _receiveContext.CancellationToken; }
        }

        public ReceiveContext ReceiveContext
        {
            get { return _receiveContext; }
        }

        public bool HasMessageType(Type messageType)
        {
            lock(_messageTypes)
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
            lock(_messageTypes)
            {
                object existing;
                if (_messageTypes.TryGetValue(typeof(T), out existing))
                {
                    message = existing as ConsumeContext<T>;
                    return message != null;
                }

                if (typeof(T) == typeof(JToken))
                {
                    _messageTypes[typeof(T)] = message = new JsonMessageConsumeContext<T>(this, _messageToken as T);
                    return true;
                }

                string typeUrn = new MessageUrn(typeof(T)).ToString();

                if (_supportedTypes.Any(typeUrn.Equals))
                {
                    object obj;
                    Type deserializeType = typeof(T);
                    if (deserializeType.IsInterface && deserializeType.IsAllowedMessageType())
                        deserializeType = InterfaceImplementationBuilder.GetProxyFor(deserializeType);

                    using (var jsonReader = _messageToken.CreateReader())
                    {
                        obj = _deserializer.Deserialize(jsonReader, deserializeType);
                    }

                    _messageTypes[typeof(T)] = message = new JsonMessageConsumeContext<T>(this, (T) obj);
                    return true;
                }

                _messageTypes[typeof(T)] = message = null;
                return false;
            }
        }

        public Task<SentContext> RespondAsync<T>(T message) where T : class
        {
            throw new NotImplementedException();
        }

        public void Respond<T>(T message) where T : class
        {
            throw new NotImplementedException();
        }

        public void RetryLater()
        {
            throw new NotImplementedException();
        }

        public IEndpoint GetEndpoint(Uri address)
        {
            throw new NotImplementedException();
        }

        public void NotifyConsumed(TimeSpan elapsed, string messageType, string consumerType)
        {
            _receiveContext.NotifyConsumed(elapsed, messageType, consumerType);
        }

        public void NotifyFaulted(string messageType, string consumerType, Exception exception)
        {
            _receiveContext.NotifyFaulted(messageType, consumerType, exception);
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
    }
}