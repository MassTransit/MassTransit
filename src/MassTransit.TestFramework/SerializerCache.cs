// Copyright 2012-2016 Chris Patterson
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
namespace MassTransit.TestFramework
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using GreenPipes.Internals.Extensions;
    using GreenPipes.Internals.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;


    public static class SerializerCache
    {
        static readonly Lazy<JsonSerializer> _serializer = new Lazy<JsonSerializer>(CreateSerializer, LazyThreadSafetyMode.PublicationOnly);

        public static JsonSerializer Serializer => _serializer.Value;

        static JsonSerializer CreateSerializer()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                TypeNameHandling = TypeNameHandling.None,
                DateParseHandling = DateParseHandling.None,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>(new JsonConverter[]
                {
                    new StringEnumConverter(),
                    new InterfaceProxyConverter(TypeCache.ImplementationBuilder)
                })
            };

            return JsonSerializer.Create(settings);
        }


        class InterfaceProxyConverter :
            JsonConverter
        {
            readonly IImplementationBuilder _builder;

            public InterfaceProxyConverter(IImplementationBuilder builder)
            {
                if (builder == null)
                    throw new ArgumentNullException(nameof(builder));
                _builder = builder;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var proxyType = _builder.GetImplementationType(objectType);

                return serializer.Deserialize(reader, proxyType);
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType.GetTypeInfo().IsInterface;
            }
        }
    }
}