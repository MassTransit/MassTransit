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


    public abstract class ReceiveEndpointBuilder
    {
        readonly IBusBuilder _builder;
        readonly IConsumePipe _consumePipe;
        readonly SerializerBuilder _serializerBuilder;

        protected ReceiveEndpointBuilder(IConsumePipe consumePipe, IBusBuilder builder)
        {
            _builder = builder;
            _consumePipe = consumePipe;
            _serializerBuilder = builder.CreateSerializerBuilder();
        }

        public IConsumePipe ConsumePipe => _consumePipe;
        public IMessageSerializer MessageSerializer => _serializerBuilder.Serializer;
        public IMessageDeserializer MessageDeserializer => _serializerBuilder.Deserializer;

        public ISendTransportProvider SendTransportProvider => _builder.SendTransportProvider;

        public void SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _serializerBuilder.SetSerializer(serializerFactory);
        }

        public void AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            _serializerBuilder.AddDeserializer(contentType, deserializerFactory);
        }

        public virtual ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe) where T : class
        {
            return _consumePipe.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer) where T : class
        {
            return _consumePipe.ConnectConsumeMessageObserver(observer);
        }

        protected ISendPipe CreateSendPipe(params ISendPipeSpecification[] specifications)
        {
            return _builder.CreateSendPipe(specifications);
        }

        protected IPublishPipe CreatePublishPipe(params IPublishPipeSpecification[] specifications)
        {
            return _builder.CreatePublishPipe(specifications);
        }
    }
}