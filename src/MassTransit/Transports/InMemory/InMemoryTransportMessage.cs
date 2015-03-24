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
namespace MassTransit.Transports.InMemory
{
    using System;
    using System.Collections.Generic;


    public class InMemoryTransportMessage
    {
        readonly byte[] _body;
        readonly IDictionary<string, object> _headers;
        readonly Guid _messageId;
        readonly string _messageType;

        public InMemoryTransportMessage(Guid messageId, byte[] body, string contentType, string messageType)
        {
            _headers = new Dictionary<string, object>();
            _messageId = messageId;
            _body = body;
            _messageType = messageType;

            _headers["MessageId"] = messageId.ToString();
            _headers["Content-Type"] = contentType;
        }

        public string MessageType
        {
            get { return _messageType; }
        }

        public Guid MessageId
        {
            get { return _messageId; }
        }

        public byte[] Body
        {
            get { return _body; }
        }

        public int DeliveryCount { get; set; }

        public IDictionary<string, object> Headers
        {
            get { return _headers; }
        }
    }
}