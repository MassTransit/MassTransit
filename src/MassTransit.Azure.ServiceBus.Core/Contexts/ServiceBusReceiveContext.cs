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
namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mime;
    using Context;
    using Microsoft.Azure.ServiceBus;


    public sealed class ServiceBusReceiveContext :
        BaseReceiveContext,
        BrokeredMessageContext
    {
        readonly Message _message;

        public ServiceBusReceiveContext(Uri inputAddress, Message message, ReceiveEndpointContext receiveEndpointContext)
            : base(inputAddress, message.SystemProperties.DeliveryCount > 1, receiveEndpointContext)
        {
            _message = message;

            GetOrAddPayload<BrokeredMessageContext>(() => this);
        }

        protected override IHeaderProvider HeaderProvider => new ServiceBusHeaderProvider(this);

        public string MessageId => _message.MessageId;

        public string CorrelationId => _message.CorrelationId;

        public TimeSpan TimeToLive => _message.TimeToLive;

        public IDictionary<string, object> Properties => _message.UserProperties;

        public int DeliveryCount => _message.SystemProperties.DeliveryCount;

        public string Label => _message.Label;

        public long SequenceNumber => _message.SystemProperties.SequenceNumber;

        public long EnqueuedSequenceNumber => _message.SystemProperties.EnqueuedSequenceNumber;

        public string LockToken => _message.SystemProperties.LockToken;

        public DateTime LockedUntil => _message.SystemProperties.LockedUntilUtc;

        public string SessionId => _message.SessionId;

        public long Size => _message.Size;

        public string To => _message.To;

        public string ReplyToSessionId => _message.ReplyToSessionId;

        public string PartitionKey => _message.PartitionKey;

        public string ViaPartitionKey => _message.ViaPartitionKey;

        public string ReplyTo => _message.ReplyTo;

        public DateTime EnqueuedTime => _message.SystemProperties.EnqueuedTimeUtc;

        public DateTime ScheduledEnqueueTime => _message.ScheduledEnqueueTimeUtc;

        public override byte[] GetBody()
        {
            return _message.Body;
        }

        public override Stream GetBodyStream()
        {
            return new MemoryStream(_message.Body, false);
        }

        protected override ContentType GetContentType()
        {
            if (!string.IsNullOrWhiteSpace(_message.ContentType))
                return new ContentType(_message.ContentType);

            return base.GetContentType();
        }
    }
}