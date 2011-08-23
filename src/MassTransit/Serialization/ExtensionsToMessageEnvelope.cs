// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Util;

    public static class ExtensionsToMessageEnvelope
    {
        public static void SetUsingMessageEnvelope(this IReceiveContext context, XmlMessageEnvelope envelope)
        {
            context.SetRequestId(envelope.RequestId);
            context.SetConversationId(envelope.ConversationId);
            context.SetCorrelationId(envelope.CorrelationId);
            context.SetSourceAddress(envelope.SourceAddress.ToUriOrNull());
            context.SetDestinationAddress(envelope.DestinationAddress.ToUriOrNull());
            context.SetResponseAddress(envelope.ResponseAddress.ToUriOrNull());
            context.SetFaultAddress(envelope.FaultAddress.ToUriOrNull());
            context.SetNetwork(envelope.Network);
            context.SetRetryCount(envelope.RetryCount);
            context.SetMessageType(envelope.MessageType);
            if (envelope.ExpirationTime.HasValue)
                context.SetExpirationTime(envelope.ExpirationTime.Value);
        }

        public static void SetUsingContext(this XmlMessageEnvelope envelope, ISendContext headers)
        {
            envelope.RequestId = headers.RequestId;
            envelope.ConversationId = headers.ConversationId;
            envelope.CorrelationId = headers.CorrelationId;
            envelope.SourceAddress = headers.SourceAddress.ToStringOrNull() ?? envelope.SourceAddress;
            envelope.DestinationAddress = headers.DestinationAddress.ToStringOrNull() ?? envelope.DestinationAddress;
            envelope.ResponseAddress = headers.ResponseAddress.ToStringOrNull() ?? envelope.ResponseAddress;
            envelope.FaultAddress = headers.FaultAddress.ToStringOrNull() ?? envelope.FaultAddress;
            envelope.Network = headers.Network;
            envelope.RetryCount = headers.RetryCount;
            envelope.MessageType = headers.MessageType ?? envelope.MessageType;
            if (headers.ExpirationTime.HasValue)
                envelope.ExpirationTime = headers.ExpirationTime.Value;
        }

        public static string ToStringOrNull(this Uri uri)
        {
            return uri == null ? null : uri.ToString();
        }

        public static Uri ToUriOrNull(this string uriString)
        {
            return string.IsNullOrEmpty(uriString) ? null : uriString.ToUri();
        }
    }
}