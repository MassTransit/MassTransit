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
    using Context;
    using Microsoft.ServiceBus.Messaging;
    using Transports;


    public class AzureServiceBusReceiveContext :
        BaseReceiveContext,
        BrokeredMessageContext
    {
        readonly BrokeredMessage _message;
        byte[] _body;

        public AzureServiceBusReceiveContext(Uri inputAddress, BrokeredMessage message)
            : base(inputAddress, message.DeliveryCount > 0)
        {
            _message = message;

            ((ReceiveContext)this).GetOrAddPayload<BrokeredMessageContext>(() => this);
        }

        protected override IHeaderProvider HeaderProvider
        {
            get { return new DictionaryHeaderProvider(_message.Properties); }
        }

        public IDictionary<string, object> Properties
        {
            get { return _message.Properties; }
        }

        public int DeliveryCount
        {
            get { return _message.DeliveryCount; }
        }

        public string Label
        {
            get { return _message.Label; }
        }

        public long SequenceNumber
        {
            get { return _message.SequenceNumber; }
        }

        public long EnqueuedSequenceNumber
        {
            get { return _message.EnqueuedSequenceNumber; }
        }

        public Guid LockToken
        {
            get { return _message.LockToken; }
        }

        public DateTime LockedUntil
        {
            get { return _message.LockedUntilUtc; }
        }

        public string SessionId
        {
            get { return _message.SessionId; }
        }

        public long Size
        {
            get { return _message.Size; }
        }

        public MessageState State
        {
            get { return _message.State; }
        }

        public bool ForcePersistence
        {
            get { return _message.ForcePersistence; }
        }

        public string To
        {
            get { return _message.To; }
        }

        public string ReplyToSessionId
        {
            get { return _message.ReplyToSessionId; }
        }

        public string PartitionKey
        {
            get { return _message.PartitionKey; }
        }

        public string ViaPartitionKey
        {
            get { return _message.ViaPartitionKey; }
        }

        public string ReplyTo
        {
            get { return _message.ReplyTo; }
        }

        public DateTime EnqueuedTime
        {
            get { return _message.EnqueuedTimeUtc; }
        }

        public DateTime ScheduledEnqueueTime
        {
            get { return _message.ScheduledEnqueueTimeUtc; }
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