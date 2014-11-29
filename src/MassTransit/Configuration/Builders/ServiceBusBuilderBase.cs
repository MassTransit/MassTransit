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
    using System;
    using System.Collections.Generic;
    using Serialization;
    using Transports;


    public abstract class ServiceBusBuilderBase
    {
        readonly Lazy<IMessageDeserializer> _deserializer;
        readonly IList<IReceiveEndpoint> _endpoints;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;
        readonly Lazy<ISendMessageSerializer> _serializer;

        protected ServiceBusBuilderBase()
        {
            _endpoints = new List<IReceiveEndpoint>();
            _deserializer = new Lazy<IMessageDeserializer>(CreateDeserializer);
            _serializer = new Lazy<ISendMessageSerializer>(CreateSerializer);
            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
        }

        protected IEnumerable<IReceiveEndpoint> ReceiveEndpoints
        {
            get { return _endpoints; }
        }

        public ISendMessageSerializer MessageSerializer
        {
            get { return _serializer.Value; }
        }

        public IMessageDeserializer MessageDeserializer
        {
            get { return _deserializer.Value; }
        }

        protected ISendEndpointProvider SendEndpointProvider
        {
            get { return _sendEndpointProvider.Value; }
        }

        JsonSendMessageSerializer CreateSerializer()
        {
            return new JsonSendMessageSerializer(JsonMessageSerializer.Serializer);
        }

        JsonMessageDeserializer CreateDeserializer()
        {
            return new JsonMessageDeserializer(JsonMessageSerializer.Deserializer, SendEndpointProvider);
        }

        public void AddReceiveEndpoint(IReceiveEndpoint receiveEndpoint)
        {
            _endpoints.Add(receiveEndpoint);
        }

        protected abstract ISendEndpointProvider CreateSendEndpointProvider();
    }
}