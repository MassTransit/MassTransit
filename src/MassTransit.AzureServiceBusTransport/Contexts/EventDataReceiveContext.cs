// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Context;
    using MassTransit.Topology;
    using Microsoft.ServiceBus.Messaging;
    using Transports;


    public sealed class EventDataReceiveContext :
        BaseReceiveContext,
        EventDataContext
    {
        readonly EventData _message;
        byte[] _body;

        public EventDataReceiveContext(Uri inputAddress, EventData message, IReceiveObserver observer, ReceiveEndpointContext receiveEndpointContext)
            : base(inputAddress, false, observer, receiveEndpointContext)
        {
            _message = message;

            GetOrAddPayload<EventDataContext>(() => this);
        }

        protected override IHeaderProvider HeaderProvider => new DictionaryHeaderProvider(_message.Properties);

        public DateTime EnqueuedTime => _message.EnqueuedTimeUtc;
        public string Offset => _message.Offset;
        public string PartitionKey => _message.PartitionKey;
        public IDictionary<string, object> Properties => _message.Properties;
        public long SequenceNumber => _message.SequenceNumber;
        public long SerializedSizeInBytes => _message.SerializedSizeInBytes;
        public IDictionary<string, object> SystemProperties => _message.SystemProperties;

        public override byte[] GetBody()
        {
            if (_body == null)
                _body = _message.GetBytes();

            return _body;
        }

        public override Stream GetBodyStream()
        {
            if (_body == null)
                _body = _message.GetBytes();

            return new MemoryStream(_body, false);
        }
    }
}