// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Custom;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;

    public class BsonMessageSerializer :
        IMessageSerializer
    {
        const string ContentTypeHeaderValue = "application/vnd.masstransit+bson";

        [ThreadStatic]
        static JsonSerializer _deserializer;

        [ThreadStatic]
        static JsonSerializer _serializer;

        static JsonSerializer Deserializer
        {
            get
            {
                return _deserializer ?? (_deserializer = JsonSerializer.Create(new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Auto,
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                        ContractResolver = new JsonContractResolver(),
                        Converters = new List<JsonConverter>(new JsonConverter[]
                            {
                                new ListJsonConverter(),
                                new InterfaceProxyConverter(),
                                new IsoDateTimeConverter{DateTimeStyles = DateTimeStyles.RoundtripKind},
                            })
                    }));
            }
        }

        static JsonSerializer Serializer
        {
            get
            {
                return _serializer ?? (_serializer = JsonSerializer.Create(new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Auto,
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                        ContractResolver = new JsonContractResolver(),
                        Converters = new List<JsonConverter>(new JsonConverter[]
                            {
                                new IsoDateTimeConverter{DateTimeStyles = DateTimeStyles.RoundtripKind},
                            }),
                    }));
            }
        }

        public string ContentType
        {
            get { return ContentTypeHeaderValue; }
        }

        public void Serialize<T>(Stream output, ISendContext<T> context)
            where T : class
        {
            context.SetContentType(ContentTypeHeaderValue);

            Envelope envelope = Envelope.Create(context);

            using (var outputStream = new NonClosingStream(output))
            using (var bsonWriter = new BsonWriter(outputStream))
            {
                Serializer.Serialize(bsonWriter, envelope);

                bsonWriter.Flush();
            }
        }

        public void Deserialize(IReceiveContext context)
        {
            Envelope result;
            using (var inputStream = new NonClosingStream(context.BodyStream))
            using (var bsonReader = new BsonReader(inputStream))
            {
                result = Deserializer.Deserialize<Envelope>(bsonReader);
            }

            context.SetUsingEnvelope(result);
            context.SetMessageTypeConverter(new JsonMessageTypeConverter(Deserializer, result.Message as JToken,
                result.MessageType));
        }
    }
}