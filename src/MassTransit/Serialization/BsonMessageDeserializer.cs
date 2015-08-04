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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;


    public class BsonMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;
        readonly IPublishEndpointProvider _publishEndpoint;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public BsonMessageDeserializer(JsonSerializer deserializer, ISendEndpointProvider sendEndpointProvider,
            IPublishEndpointProvider publishEndpoint)
        {
            _deserializer = deserializer;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("bson");
            scope.Add("contentType", BsonMessageSerializer.BsonContentType.MediaType);
        }

        ContentType IMessageDeserializer.ContentType => BsonMessageSerializer.BsonContentType;

        ConsumeContext IMessageDeserializer.Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                MessageEnvelope envelope;
                using (Stream body = receiveContext.GetBody())
                using (var jsonReader = new BsonReader(body))
                {
                    envelope = _deserializer.Deserialize<MessageEnvelope>(jsonReader);
                }

                return new JsonConsumeContext(_deserializer, _sendEndpointProvider, _publishEndpoint, receiveContext, envelope);
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