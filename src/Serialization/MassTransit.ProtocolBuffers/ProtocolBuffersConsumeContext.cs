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
namespace MassTransit.ProtocolBuffers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Context;
    using ProtoBuf;
    using Serialization;
    using Util;


    public class ProtocolBuffersConsumeContext :
        BaseConsumeContext
    {
        readonly ProtocolBuffersMessageEnvelope _envelope;
        readonly long _offset;
        readonly IDictionary<Type, object> _messageTypes;
        readonly IObjectTypeDeserializer _objectTypeDeserializer;
        readonly string[] _supportedTypes;
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

        public ProtocolBuffersConsumeContext(ISendEndpointProvider sendEndpointProvider,
            IPublishEndpointProvider publishEndpointProvider, ReceiveContext receiveContext, ProtocolBuffersMessageEnvelope envelope, long offset)
            : base(receiveContext, sendEndpointProvider, publishEndpointProvider)
        {
            _envelope = envelope;
            _offset = offset;
            _supportedTypes = envelope.MessageType.ToArray();
            _messageTypes = new Dictionary<Type, object>();
            _objectTypeDeserializer = null;
        }

        public override Guid? MessageId => _messageId.HasValue ? _messageId : (_messageId = ConvertIdToGuid(_envelope.MessageId));
        public override Guid? RequestId => _requestId.HasValue ? _requestId : (_requestId = ConvertIdToGuid(_envelope.RequestId));
        public override Guid? CorrelationId => _correlationId.HasValue ? _correlationId : (_correlationId = ConvertIdToGuid(_envelope.CorrelationId));
        public override Guid? ConversationId => _conversationId.HasValue ? _conversationId : (_conversationId = ConvertIdToGuid(_envelope.ConversationId));
        public override Guid? InitiatorId => _initiatorId.HasValue ? _initiatorId : (_initiatorId = ConvertIdToGuid(_envelope.InitiatorId));
        public override DateTime? ExpirationTime => _envelope.ExpirationTime;
        public override Uri SourceAddress => _sourceAddress ?? (_sourceAddress = ConvertToUri(_envelope.SourceAddress));
        public override Uri DestinationAddress => _destinationAddress ?? (_destinationAddress = ConvertToUri(_envelope.DestinationAddress));
        public override Uri ResponseAddress => _responseAddress ?? (_responseAddress = ConvertToUri(_envelope.ResponseAddress));
        public override Uri FaultAddress => _faultAddress ?? (_faultAddress = ConvertToUri(_envelope.FaultAddress));
        public override Headers Headers => _headers ?? (_headers = new ProtocolBuffersMessageHeaders(_objectTypeDeserializer, _envelope.Headers));
        public override HostInfo Host => _envelope.Host;
        public override IEnumerable<string> SupportedMessageTypes => _supportedTypes;

        public override bool HasMessageType(Type messageType)
        {
            lock (_messageTypes)
            {
                object existing;
                if (_messageTypes.TryGetValue(messageType, out existing))
                    return existing != null;
            }

            var typeUrn = new MessageUrn(messageType).ToString();

            return _supportedTypes.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase));
        }

        public override bool TryGetMessage<T>(out ConsumeContext<T> message)
        {
            lock (_messageTypes)
            {
                object existing;
                if (_messageTypes.TryGetValue(typeof(T), out existing))
                {
                    message = existing as ConsumeContext<T>;
                    return message != null;
                }

                var typeUrn = new MessageUrn(typeof(T)).ToString();

                if (_supportedTypes.Any(typeUrn.Equals))
                {
                    using (var stream = ReceiveContext.GetBody())
                    {
                        stream.Seek(_offset, SeekOrigin.Current);

                        _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, ProtoBuf.Serializer.DeserializeWithLengthPrefix<T>(stream, PrefixStyle.Fixed32));
                        return true;
                    }
                }

                _messageTypes[typeof(T)] = message = null;
                return false;
            }
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