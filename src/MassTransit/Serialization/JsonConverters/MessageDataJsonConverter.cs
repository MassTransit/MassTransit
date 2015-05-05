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
namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Linq;
    using Internals.Extensions;
    using MessageData;
    using Newtonsoft.Json;
    using Util;


    public class MessageDataJsonConverter :
        JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var messageData = value as IMessageData;
            if (messageData == null)
                return;

            var reference = new MessageDataReference {Reference = messageData.Address};

            serializer.Serialize(writer, reference);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type dataType = objectType.GetClosingArguments(typeof(MessageData<>)).First();

            var reference = serializer.Deserialize<MessageDataReference>(reader);
            if (reference == null || reference.Reference == null)
                return Activator.CreateInstance(typeof(EmptyMessageData<>).MakeGenericType(dataType));

            if (dataType == typeof(string))
                return new DeserializedMessageData<string>(reference.Reference);
            if (dataType == typeof(byte[]))
                return new DeserializedMessageData<byte[]>(reference.Reference);

            throw new MessageDataException("The message data type was unknown: " + TypeMetadataCache.ShortName(dataType));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.HasInterface<IMessageData>();
        }
    }
}