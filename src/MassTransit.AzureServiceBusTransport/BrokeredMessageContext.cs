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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ServiceBus.Messaging;


    public class BrokeredMessageContext :
        AzureServiceBusMessageContext
    {
        readonly BrokeredMessage _message;

        public BrokeredMessageContext(BrokeredMessage message)
        {
            _message = message;
        }

        public int DeliveryCount
        {
            get { return _message.DeliveryCount; }
        }

        public string Label
        {
            get { return _message.Label; }
            set { _message.Label = value; }
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
            set { _message.To = value; }
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

        public IDictionary<string, object> Properties
        {
            get { return _message.Properties; }
        }
    }
}