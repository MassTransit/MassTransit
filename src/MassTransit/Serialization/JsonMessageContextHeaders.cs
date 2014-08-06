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
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class JsonMessageContextHeaders :
        ContextHeaders
    {
        readonly JsonSerializer _deserializer;
        readonly IDictionary<string, object> _headers;

        public JsonMessageContextHeaders(JsonSerializer deserializer, IDictionary<string, object> headers)
        {
            _deserializer = deserializer;
            _headers = headers;
        }

        public T Get<T>(string key, T defaultValue = default(T))
            where T : class
        {
            if (key == null)
                throw new ArgumentNullException("key");

            object obj;
            if (!_headers.TryGetValue(key, out obj))
                return defaultValue;

            if (obj == null)
                return defaultValue;

            var token = obj as JToken ?? new JValue(obj);

            if (token.Type == JTokenType.Null)
                return defaultValue;

            using (JsonReader jsonReader = token.CreateReader())
                return (T)_deserializer.Deserialize(jsonReader, typeof(T)) ?? defaultValue;
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : struct
        {
            if (key == null)
                throw new ArgumentNullException("key");

            object obj;
            if (!_headers.TryGetValue(key, out obj))
                return defaultValue;

            if (obj == null)
                return defaultValue;

            var token = obj as JToken ?? new JValue(obj);

            if (token.Type == JTokenType.Null)
                token = new JObject();

            if (token.Type == JTokenType.Null)
                return defaultValue;

            using (JsonReader jsonReader = token.CreateReader())
                return (T)_deserializer.Deserialize(jsonReader, typeof(T));
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return _headers.TryGetValue(key, out value);
        }
    }
}