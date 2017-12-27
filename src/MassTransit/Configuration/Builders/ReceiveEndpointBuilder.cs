// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Net.Mime;
    using EndpointSpecifications;
    using GreenPipes;
    using Pipeline;


    public abstract class ReceiveEndpointBuilder
    {
        readonly IConsumePipe _consumePipe;
        readonly SerializerBuilder _serializerBuilder;
        readonly IEndpointConfiguration _configuration;

        protected ReceiveEndpointBuilder(IBusBuilder builder, IEndpointConfiguration configuration)
        {
            _configuration = configuration;

            _consumePipe = configuration.Consume.CreatePipe();

            _serializerBuilder = builder.CreateSerializerBuilder();
        }

        public IConsumePipe ConsumePipe => _consumePipe;
        public IMessageSerializer MessageSerializer => _serializerBuilder.Serializer;
        public IMessageDeserializer MessageDeserializer => _serializerBuilder.Deserializer;

        public void SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _serializerBuilder.SetSerializer(serializerFactory);
        }

        public void AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            _serializerBuilder.AddDeserializer(contentType, deserializerFactory);
        }

        public virtual ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            IPipe<ConsumeContext<T>> messagePipe = _configuration.Consume.Specification.GetMessageSpecification<T>().BuildMessagePipe(pipe);

            return _consumePipe.ConnectConsumePipe(messagePipe);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            return _consumePipe.ConnectConsumeMessageObserver(observer);
        }
    }
}