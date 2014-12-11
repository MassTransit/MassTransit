// Copyright 2007-2014 Chris Patterson
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
namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using MassTransit.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;


    public static class SerializerCache
    {
        static readonly Lazy<JsonSerializer> _deserializer = new Lazy<JsonSerializer>(CreateDeserializer,
            LazyThreadSafetyMode.PublicationOnly);

        static readonly Lazy<JsonSerializer> _serializer = new Lazy<JsonSerializer>(CreateSerializer, LazyThreadSafetyMode.PublicationOnly);

        public static JsonSerializer Serializer
        {
            get { return _serializer.Value; }
        }

        public static JsonSerializer Deserializer
        {
            get { return _deserializer.Value; }
        }

        static JsonSerializer CreateSerializer()
        {
            JsonSerializerSettings source = JsonMessageSerializer.SerializerSettings;

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = source.NullValueHandling,
                DefaultValueHandling = source.DefaultValueHandling,
                MissingMemberHandling = source.MissingMemberHandling,
                ObjectCreationHandling = source.ObjectCreationHandling,
                ConstructorHandling = source.ConstructorHandling,
                ContractResolver = source.ContractResolver,
                DateParseHandling = source.DateParseHandling,
                Converters = new List<JsonConverter>(source.Converters)
            };

            settings.Converters.Add(new StringEnumConverter());

            return JsonSerializer.Create(settings);
        }

        static JsonSerializer CreateDeserializer()
        {
            JsonSerializerSettings source = JsonMessageSerializer.DeserializerSettings;

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = source.NullValueHandling,
                DefaultValueHandling = source.DefaultValueHandling,
                MissingMemberHandling = source.MissingMemberHandling,
                ObjectCreationHandling = source.ObjectCreationHandling,
                ConstructorHandling = source.ConstructorHandling,
                ContractResolver = source.ContractResolver,
                DateParseHandling = source.DateParseHandling,
                Converters = new List<JsonConverter>(source.Converters)
            };

            settings.Converters.Add(new StringEnumConverter());

            return JsonSerializer.Create(settings);
        }
    }
}