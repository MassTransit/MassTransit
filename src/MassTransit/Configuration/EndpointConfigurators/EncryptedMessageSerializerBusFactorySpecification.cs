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
namespace MassTransit.EndpointConfigurators
{
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using Serialization;


    public class EncryptedMessageSerializerBusFactorySpecification :
        IBusFactorySpecification
    {
        readonly ICryptoStreamProvider _streamProvider;

        public EncryptedMessageSerializerBusFactorySpecification(ICryptoStreamProvider streamProvider)
        {
            _streamProvider = streamProvider;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Configure(IBusBuilder builder)
        {
            builder.SetMessageSerializer(CreateSerializer);

            builder.AddMessageDeserializer(EncryptedMessageSerializer.EncryptedContentType, CreateDeserializer);
        }

        IMessageDeserializer CreateDeserializer(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            return new EncryptedMessageDeserializer(BsonMessageSerializer.Deserializer, sendEndpointProvider, publishEndpoint,
                _streamProvider);
        }

        IMessageSerializer CreateSerializer()
        {
            ICryptoStreamProvider streamProvider = GetStreamProvider();

            return new EncryptedMessageSerializer(streamProvider);
        }

        ICryptoStreamProvider GetStreamProvider()
        {
            return _streamProvider;
        }
    }
}