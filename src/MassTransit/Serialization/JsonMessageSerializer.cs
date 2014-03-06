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
    using System.Runtime.Serialization;
    using Custom;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;

    public class JsonMessageSerializer :
        IMessageSerializer
    {
        const string ContentTypeHeaderValue = "application/vnd.masstransit+json";

        [ThreadStatic]
        static JsonSerializer _deserializer;

        [ThreadStatic]
        static JsonSerializer _serializer;

        public static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Auto,
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                        ContractResolver = new JsonContractResolver(),
                        DateParseHandling = DateParseHandling.None,
                        Converters = new List<JsonConverter>(new JsonConverter[]
                            {
                                new ByteArrayConverter(), 
                                new IsoDateTimeConverter{DateTimeStyles = DateTimeStyles.RoundtripKind},
                            }),
                    };

        public static JsonSerializerSettings DeserializerSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Auto,
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                        ContractResolver = new JsonContractResolver(),
                        DateParseHandling = DateParseHandling.None,
                        Converters = new List<JsonConverter>(new JsonConverter[]
                            {
                                new ByteArrayConverter(), 
                                new ListJsonConverter(),
                                new InterfaceProxyConverter(),
                                new StringDecimalConverter(),
                                new IsoDateTimeConverter{DateTimeStyles = DateTimeStyles.RoundtripKind},
                            })
                    };


        public static JsonSerializer Deserializer
        {
            get
            {
                return _deserializer ?? (_deserializer = JsonSerializer.Create(DeserializerSettings));
            }
        }

        public static JsonSerializer Serializer
        {
            get
            {
                return _serializer ?? (_serializer = JsonSerializer.Create(SerializerSettings));
            }
        }

        public string ContentType
        {
            get { return ContentTypeHeaderValue; }
        }

        public void Serialize<T>(Stream output, ISendContext<T> context)
            where T : class
        {
            try
            {
                context.SetContentType(ContentTypeHeaderValue);

                Envelope envelope = Envelope.Create(context);

                using (var nonClosingStream = new NonClosingStream(output))
                using (var writer = new StreamWriter(nonClosingStream))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jsonWriter.Formatting = Formatting.Indented;

                    Serializer.Serialize(jsonWriter, envelope);

                    jsonWriter.Flush();
                    writer.Flush();
                }
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

        public void Deserialize(IReceiveContext context)
        {
            try
            {
                Envelope result;
                using (var nonClosingStream = new NonClosingStream(context.BodyStream))
                using (var reader = new StreamReader(nonClosingStream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    result = Deserializer.Deserialize<Envelope>(jsonReader);
                }

                context.SetUsingEnvelope(result);
                context.SetMessageTypeConverter(new JsonMessageTypeConverter(Deserializer, result.Message as JToken,
                    result.MessageType));
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to deserialize message", ex);
            }
        }
    }
}