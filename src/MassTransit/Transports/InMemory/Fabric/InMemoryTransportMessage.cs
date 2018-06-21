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
namespace MassTransit.Transports.InMemory.Fabric
{
    using System;
    using System.Collections.Generic;


    public class InMemoryTransportMessage
    {
        public InMemoryTransportMessage(Guid messageId, byte[] body, string contentType, string messageType)
        {
            Headers = new Dictionary<string, object>();
            MessageId = messageId;
            Body = body;
            MessageType = messageType;

            Headers["MessageId"] = messageId.ToString();
            Headers["Content-Type"] = contentType;
        }

        public string MessageType { get; }

        public Guid MessageId { get; }

        public byte[] Body { get; }

        public int DeliveryCount { get; set; }

        public IDictionary<string, object> Headers { get; }
    }
}