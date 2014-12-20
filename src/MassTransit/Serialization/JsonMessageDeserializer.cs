// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Runtime.Serialization;
    using System.Text;
    using Newtonsoft.Json;


    public class JsonMessageDeserializer :
        IMessageDeserializer
    {
        readonly JsonSerializer _deserializer;
        readonly IPublishEndpoint _publishEndpoint;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public JsonMessageDeserializer(JsonSerializer deserializer, ISendEndpointProvider sendEndpointProvider,
            IPublishEndpoint publishEndpoint)
        {
            _deserializer = deserializer;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        ConsumeContext IMessageDeserializer.Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                MessageEnvelope envelope;
                using (Stream body = receiveContext.Body)
                using (var reader = new StreamReader(body, Encoding.UTF8, false, 1024, true))
                using (var jsonReader = new JsonTextReader(reader))
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