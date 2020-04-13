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
namespace MassTransit
{
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;
    using Serialization;


    public static class DeserializeVariableExtensions
    {
        public static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            object obj;
            if (!dictionary.TryGetValue(key, out obj))
            {
                value = default(T);
                return false;
            }

            value = Deserialize<T>(obj);
            return true;
        }

        public static bool TryGetValueCaseInsensitive(this IDictionary<string, object> dictionary, string key, out object value)
        {
            object obj;
            if (!dictionary.TryGetValue(key, out obj) && !TryGetValueCamelCase(key, dictionary, out obj))
            {
                value = default;
                return false;
            }

            value = obj;
            return true;
        }

        public static bool TryGetValueCaseInsensitive<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            object obj;
            if (!dictionary.TryGetValueCaseInsensitive(key, out obj))
            {
                value = default(T);
                return false;
            }

            value = Deserialize<T>(obj);
            return true;
        }

        static T Deserialize<T>(object obj)
        {
            var token = obj as JToken ?? new JValue(obj);

            if (token.Type == JTokenType.Null)
                token = new JObject();

            using (var jsonReader = new JTokenReader(token))
            {
                return (T)JsonMessageSerializer.Deserializer.Deserialize(jsonReader, typeof(T));
            }
        }

        static bool TryGetValueCamelCase(string key, IDictionary<string, object> dictionary, out object value)
        {
            if (char.IsUpper(key[0]))
            {
                char[] chars = key.ToCharArray();
                chars[0] = char.ToLower(chars[0]);

                key = new string(chars);
                return dictionary.TryGetValue(key, out value);
            }

            value = null;
            return false;
        }
    }
}
