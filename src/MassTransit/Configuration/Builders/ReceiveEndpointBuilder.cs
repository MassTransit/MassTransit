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
    using System.Net.Mime;
    using GreenPipes;
    using Pipeline;


    public abstract class ReceiveEndpointBuilder
    {
        readonly IBusBuilder _builder;

        protected ReceiveEndpointBuilder(IConsumePipe consumePipe, IBusBuilder builder)
        {
            _builder = builder;
            ConsumePipe = consumePipe;
        }

        protected IConsumePipe ConsumePipe { get; }

        public void SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _builder.SetMessageSerializer(serializerFactory);
        }

        public void AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            _builder.AddMessageDeserializer(contentType, deserializerFactory);
        }

        public virtual ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe) where T : class
        {
            return ConsumePipe.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer) where T : class
        {
            return ConsumePipe.ConnectConsumeMessageObserver(observer);
        }
    }
}