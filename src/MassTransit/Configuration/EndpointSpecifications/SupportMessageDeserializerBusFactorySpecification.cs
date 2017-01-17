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
    using System.Net.Mime;
    using Builders;
    using GreenPipes;


    public class SupportMessageDeserializerBusFactorySpecification :
        IBusFactorySpecification
    {
        readonly ContentType _contentType;
        readonly DeserializerFactory _deserializerFactory;

        public SupportMessageDeserializerBusFactorySpecification(ContentType contentType, DeserializerFactory deserializerFactory)
        {
            _contentType = contentType;
            _deserializerFactory = deserializerFactory;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_contentType == null)
                yield return this.Failure("ContentType", "must not be null");
            else
            {
                if (string.IsNullOrWhiteSpace(_contentType.MediaType))
                    yield return this.Failure("MediaType", "must be specified");
            }

            if (_deserializerFactory == null)
                yield return this.Failure("DeserializerFactory", "must not be null");
        }

        public void Apply(IBusBuilder builder)
        {
            builder.AddMessageDeserializer(_contentType, _deserializerFactory);
        }
    }
}