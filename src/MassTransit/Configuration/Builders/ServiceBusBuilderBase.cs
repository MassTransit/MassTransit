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
namespace MassTransit.Builders
{
    using System.Collections.Generic;
    using Serialization;
    using Transports;


    public abstract class ServiceBusBuilderBase
    {
        readonly IMessageDeserializer _messageDeserializer;
        readonly ISendMessageSerializer _messageSerializer;
        readonly IList<IReceiveEndpoint> _receiveEndpoints;

        protected ServiceBusBuilderBase()
        {
            _receiveEndpoints = new List<IReceiveEndpoint>();

            _messageDeserializer = new JsonMessageDeserializer(JsonMessageSerializer.Deserializer);
            _messageSerializer = new JsonSendMessageSerializer(JsonMessageSerializer.Serializer);
        }

        protected IEnumerable<IReceiveEndpoint> ReceiveEndpoints
        {
            get { return _receiveEndpoints; }
        }

        public ISendMessageSerializer MessageSerializer
        {
            get { return _messageSerializer; }
        }

        public IMessageDeserializer MessageDeserializer
        {
            get { return _messageDeserializer; }
        }

        public void AddReceiveEndpoint(IReceiveEndpoint receiveEndpoint)
        {
            _receiveEndpoints.Add(receiveEndpoint);
        }
    }
}