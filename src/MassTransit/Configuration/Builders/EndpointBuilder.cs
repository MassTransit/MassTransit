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


    public abstract class EndpointBuilder :
        IBusBuilder
    {
        readonly IBusBuilder _builder;
        readonly SerializerBuilder _serializerBuilder;

        protected EndpointBuilder(IBusBuilder builder)
        {
            _builder = builder;
            _serializerBuilder = builder.CreateSerializerBuilder();
        }

        public ISendTransportProvider SendTransportProvider => _builder.SendTransportProvider;

        public IMessageSerializer MessageSerializer => _serializerBuilder.Serializer;

        public IMessageDeserializer MessageDeserializer => _serializerBuilder.Deserializer;

        public SerializerBuilder CreateSerializerBuilder()
        {
            return new SerializerBuilder(_serializerBuilder);
        }

        public abstract ISendEndpointProvider CreateSendEndpointProvider(Uri sourceAddress, params ISendPipeSpecification[] specifications);

        public abstract IPublishEndpointProvider CreatePublishEndpointProvider(Uri sourceAddress, params IPublishPipeSpecification[] specifications);

        public void SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _serializerBuilder.SetSerializer(serializerFactory);
        }

        public void AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            _serializerBuilder.AddDeserializer(contentType, deserializerFactory);
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

        public IPublishPipe CreatePublishPipe(params IPublishPipeSpecification[] specifications)
        {
            return _builder.CreatePublishPipe(specifications);
        }
    }
}