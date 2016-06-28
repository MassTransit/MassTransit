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
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using ProtoBuf;
    using Util;


    public class ProtocolBuffersMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/vnd.masstransit+proto";
        public static readonly ContentType ProtocolBuffersContentType = new ContentType(ContentTypeHeaderValue);

        static ProtocolBuffersMessageSerializer()
        {
            AddBuiltInTypes();
        }

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            try
            {
                context.ContentType = ProtocolBuffersContentType;

                var envelope = new ProtocolBuffersMessageEnvelope(context, TypeMetadataCache<T>.MessageTypeNames);

                Serializer.SerializeWithLengthPrefix(stream, envelope, PrefixStyle.Fixed32);
                Serializer.SerializeWithLengthPrefix(stream, context.Message, PrefixStyle.Fixed32);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize message", ex);
            }
        }

        public ContentType ContentType => ProtocolBuffersContentType;

        static void AddBuiltInTypes()
        {
            Serializer.PrepareSerializer<BusHostInfo>();
        }
    }
}