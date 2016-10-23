// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Net.Mime;
    using GreenPipes;
    using Pipeline;


    public class EndpointBuilder :
        IBusBuilder
    {
        readonly IBusBuilder _builder;

        IReceiveEndpoint _receiveEndpoint;

        protected EndpointBuilder(IBusBuilder builder)
        {
            _builder = builder;
        }

        public IReceiveEndpoint ReceiveEndpoint => _receiveEndpoint;

        public ISendTransportProvider SendTransportProvider => _builder.SendTransportProvider;

        public IMessageDeserializer GetMessageDeserializer(ISendEndpointProvider sendEndpointProvider, IPublishEndpointProvider publishEndpointProvider)
        {
            return _builder.GetMessageDeserializer(sendEndpointProvider, publishEndpointProvider);
        }

        public ISendEndpointProvider CreateSendEndpointProvider(Uri sourceAddress, params ISendPipeSpecification[] specifications)
        {
            return _builder.CreateSendEndpointProvider(sourceAddress, specifications);
        }

        public IPublishEndpointProvider CreatePublishEndpointProvider(Uri sourceAddress, params IPublishPipeSpecification[] specifications)
        {
            return _builder.CreatePublishEndpointProvider(sourceAddress, specifications);
        }

        public void AddReceiveEndpoint(string queueName, IReceiveEndpoint receiveEndpoint)
        {
            if (_receiveEndpoint != null)
                throw new ConfigurationException("The receive endpoint with the same queue name is already configured");

            _builder.AddReceiveEndpoint(queueName, receiveEndpoint);

            _receiveEndpoint = receiveEndpoint;
        }

        public void SetMessageSerializer(Func<IMessageSerializer> serializerFactory)
        {
            _builder.SetMessageSerializer(serializerFactory);
        }

        public void AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            _builder.AddMessageDeserializer(contentType, deserializerFactory);
        }

        public IConsumePipe CreateConsumePipe(params IConsumePipeSpecification[] specifications)
        {
            return _builder.CreateConsumePipe(specifications);
        }

        public ISendPipe CreateSendPipe(params ISendPipeSpecification[] specifications)
        {
            return _builder.CreateSendPipe(specifications);
        }

        ConnectHandle IBusBuilder.ConnectBusObserver(IBusObserver observer)
        {
            return _builder.ConnectBusObserver(observer);
        }
    }
}