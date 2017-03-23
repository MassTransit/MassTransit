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
namespace MassTransit.Serialization
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using GreenPipes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    using Util;


    public class EncryptedMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;
        readonly IObjectTypeDeserializer _objectTypeDeserializer;
        readonly ICryptoStreamProvider _provider;

        public EncryptedMessageDeserializer(JsonSerializer deserializer, ICryptoStreamProvider provider)
        {
            _deserializer = deserializer;
            _provider = provider;

            _objectTypeDeserializer = new ObjectTypeDeserializer(_deserializer);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("encrypted");
            scope.Add("contentType", EncryptedMessageSerializer.EncryptedContentType.MediaType);
            _provider.Probe(scope);
        }

        public ContentType ContentType => EncryptedMessageSerializer.EncryptedContentType;

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                MessageEnvelope envelope;
                using (Stream body = receiveContext.GetBody())
                using (Stream cryptoStream = _provider.GetDecryptStream(body, receiveContext))
                using (var jsonReader = new BsonDataReader(cryptoStream))
                {
                    envelope = _deserializer.Deserialize<MessageEnvelope>(jsonReader);
                }

                return new JsonConsumeContext(_deserializer, _objectTypeDeserializer, receiveContext, envelope);
            }
            catch (JsonSerializationException ex)
            {
                throw new SerializationException("A JSON serialization exception occurred while deserializing the message envelope", ex);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("An exception occurred while deserializing the message envelope", ex);
            }
        }
    }
}