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
namespace MassTransit.Context
{
    using System;

    public abstract class MessageContext :
        IMessageContext
    {
        MessageHeaders _headers;

        public MessageContext()
        {
            _headers = new MessageHeaders();
        }

        public string MessageId { get; private set; }
        public string MessageType { get; private set; }
        public string ContentType { get; private set; }
        public string RequestId { get; private set; }
        public string ConversationId { get; private set; }
        public string CorrelationId { get; private set; }
        public Uri SourceAddress { get; private set; }
        public Uri InputAddress { get; private set; }
        public Uri DestinationAddress { get; private set; }
        public Uri ResponseAddress { get; private set; }
        public Uri FaultAddress { get; private set; }
        public string Network { get; private set; }
        public DateTime? ExpirationTime { get; private set; }
        public int RetryCount { get; private set; }

        public IMessageHeaders Headers
        {
            get { return _headers; }
        }

        public string OriginalMessageId { get; private set; }

        public void SetOriginalMessageId(string value)
        {
            OriginalMessageId = value;
        }

        public void SetMessageId(string value)
        {
            MessageId = value;
        }

        public void SetRequestId(string value)
        {
            RequestId = value;
        }

        public void SetConversationId(string value)
        {
            ConversationId = value;
        }

        public void SetCorrelationId(string value)
        {
            CorrelationId = value;
        }

        public void SetMessageType(string value)
        {
            MessageType = value;
        }

        public void SetSourceAddress(Uri value)
        {
            SourceAddress = value;
        }

        public void SetInputAddress(Uri value)
        {
            InputAddress = value;
        }

        public void SetDestinationAddress(Uri value)
        {
            DestinationAddress = value;
        }

        public void SetResponseAddress(Uri value)
        {
            ResponseAddress = value;
        }

        public void SetFaultAddress(Uri value)
        {
            FaultAddress = value;
        }

        public void SetNetwork(string value)
        {
            Network = value;
        }

        public void SetExpirationTime(DateTime value)
        {
            ExpirationTime = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        }

        public void SetRetryCount(int value)
        {
            RetryCount = value;
        }

        public void SetContentType(string value)
        {
            ContentType = value;
        }

        public void SetHeader(string key, string value)
        {
            _headers[key] = value;
        }

        public void SetUsing(IMessageContext context)
        {
            SetMessageType(context.MessageType);
            SetMessageId(context.MessageId);
            SetRequestId(context.RequestId);
            SetConversationId(context.ConversationId);
            SetCorrelationId(context.CorrelationId);
            SetSourceAddress(context.SourceAddress);
            SetDestinationAddress(context.DestinationAddress);
            SetResponseAddress(context.ResponseAddress);
            SetFaultAddress(context.FaultAddress);
            SetNetwork(context.Network);
            if (context.ExpirationTime.HasValue)
                SetExpirationTime(context.ExpirationTime.Value);
            SetRetryCount(context.RetryCount);
            SetContentType(context.ContentType);

            _headers = new MessageHeaders(context.Headers);
        }
    }
}