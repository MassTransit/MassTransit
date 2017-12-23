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
namespace MassTransit.EndpointSpecifications
{
    using System.Collections.Generic;
    using Builders;
    using GreenPipes;
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

        public void Apply(IBusBuilder builder)
        {
            builder.SetMessageSerializer(CreateSerializer);

            builder.AddMessageDeserializer(EncryptedMessageSerializer.EncryptedContentType, CreateDeserializer);
        }

        IMessageDeserializer CreateDeserializer()
        {
            return new EncryptedMessageDeserializer(BsonMessageSerializer.Deserializer, _streamProvider);
        }

        IMessageSerializer CreateSerializer()
        {
            var streamProvider = GetStreamProvider();

            return new EncryptedMessageSerializer(streamProvider);
        }

        ICryptoStreamProvider GetStreamProvider()
        {
            return _streamProvider;
        }
    }
}