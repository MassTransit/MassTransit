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
namespace MassTransit.ProtocolBuffers
{
    using System;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using ProtoBuf;


    public class ProtocolBuffersMessageDeserializer :
        IMessageDeserializer
    {
        readonly IPublishEndpointProvider _publishEndpointProvider;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public ProtocolBuffersMessageDeserializer(ISendEndpointProvider sendEndpointProvider, IPublishEndpointProvider publishEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpointProvider = publishEndpointProvider;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("json");
            scope.Add("contentType", ProtocolBuffersMessageSerializer.ProtocolBuffersContentType.MediaType);
        }

        ContentType IMessageDeserializer.ContentType => ProtocolBuffersMessageSerializer.ProtocolBuffersContentType;

        ConsumeContext IMessageDeserializer.Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                ProtocolBuffersMessageEnvelope envelope;
                long offset;
                using (var body = receiveContext.GetBody())
                {
                    envelope = Serializer.DeserializeWithLengthPrefix<ProtocolBuffersMessageEnvelope>(body, PrefixStyle.Fixed32);

                    offset = body.Position;
                }

                return new ProtocolBuffersConsumeContext(_sendEndpointProvider, _publishEndpointProvider, receiveContext, envelope, offset);
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