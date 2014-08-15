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
namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;


    public class InMemoryTransportMessage
    {
        readonly IDictionary<string, object> _headers;
        readonly Guid _messageId;

        public InMemoryTransportMessage(Guid messageId, byte[] body, string contentType)
        {
            _messageId = messageId;
            _headers = new Dictionary<string, object>();
            Body = body;

            _headers["MessageId"] = messageId.ToString();
            _headers["Content-Type"] = contentType;
        }

        public byte[] Body { get; private set; }

        public Guid MessageId
        {
            get { return _messageId; }
        }

        public int DeliveryCount { get; set; }

        public IDictionary<string, object> Headers
        {
            get { return _headers; }
        }
    }
}