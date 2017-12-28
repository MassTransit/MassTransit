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
namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Topology;
    using Microsoft.ServiceBus.Messaging;
    using Transports;


    public sealed class ServiceBusReceiveContext :
        BaseReceiveContext,
        BrokeredMessageContext
    {
        readonly BrokeredMessage _message;
        byte[] _body;

        public ServiceBusReceiveContext(Uri inputAddress, BrokeredMessage message, IReceiveObserver observer, IReceiveEndpointTopology topology)
            : base(inputAddress, message.DeliveryCount > 1, observer, topology)
        {
            _message = message;

            GetOrAddPayload<BrokeredMessageContext>(() => this);
        }

        protected override IHeaderProvider HeaderProvider => new DictionaryHeaderProvider(_message.Properties);

        public string MessageId => _message.MessageId;

        public string CorrelationId => _message.CorrelationId;

        public TimeSpan TimeToLive => _message.TimeToLive;

        public IDictionary<string, object> Properties => _message.Properties;

        public int DeliveryCount => _message.DeliveryCount;

        public string Label => _message.Label;

        public long SequenceNumber => _message.SequenceNumber;

        public long EnqueuedSequenceNumber => _message.EnqueuedSequenceNumber;

        public Guid LockToken => _message.LockToken;

        public DateTime LockedUntil => _message.LockedUntilUtc;

        public string SessionId => _message.SessionId;

        public long Size => _message.Size;

        public MessageState State => _message.State;

        public bool ForcePersistence => _message.ForcePersistence;

        public string To => _message.To;

        public string ReplyToSessionId => _message.ReplyToSessionId;

        public string PartitionKey => _message.PartitionKey;

        public string ViaPartitionKey => _message.ViaPartitionKey;

        public string ReplyTo => _message.ReplyTo;

        public DateTime EnqueuedTime => _message.EnqueuedTimeUtc;

        public DateTime ScheduledEnqueueTime => _message.ScheduledEnqueueTimeUtc;

        public Task RenewLockAsync()
        {
            return _message.RenewLockAsync();
        }

        protected override Stream GetBodyStream()
        {
            if (_body == null)
            {
                using (var bodyStream = _message.GetBody<Stream>())
                using (var ms = new MemoryStream())
                {
                    bodyStream.CopyTo(ms);

                    _body = ms.ToArray();
                }
            }

            return new MemoryStream(_body, false);
        }

        protected override ContentType GetContentType()
        {
            if (!string.IsNullOrWhiteSpace(_message.ContentType))
                return new ContentType(_message.ContentType);

            return base.GetContentType();
        }
        
        
    }
}