// Copyright 2007-2013 Chris Patterson
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
    using Contracts;
    using MassTransit.Serialization;
    using Newtonsoft.Json.Linq;


    public static class RoutingSlipEventExtensions
    {
        public static T GetResult<T>(this RoutingSlipActivityCompensated source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializeVariable<T>(key, source.Data);
        }

        public static T GetVariable<T>(this RoutingSlipActivityCompensated source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializeVariable<T>(key, source.Variables);
        }

        public static T GetResult<T>(this RoutingSlipActivityCompensationFailed source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializeVariable<T>(key, source.Data);
        }

        public static T GetVariable<T>(this RoutingSlipActivityCompensationFailed source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializeVariable<T>(key, source.Variables);
        }

        public static T GetArgument<T>(this RoutingSlipActivityCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializeVariable<T>(key, source.Arguments);
        }

        public static T GetResult<T>(this RoutingSlipActivityCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializeVariable<T>(key, source.Data);
        }

        public static T GetVariable<T>(this RoutingSlipActivityCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializeVariable<T>(key, source.Variables);
        }

        public static T GetArgument<T>(this RoutingSlipActivityFaulted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializeVariable<T>(key, source.Arguments);
        }

        public static T GetVariable<T>(this RoutingSlipActivityFaulted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializeVariable<T>(key, source.Variables);
        }

        public static T GetVariable<T>(this RoutingSlipCompensationFailed source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializeVariable<T>(key, source.Variables);
        }

        public static T GetVariable<T>(this RoutingSlipCompleted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializeVariable<T>(key, source.Variables);
        }

        public static T GetVariable<T>(this RoutingSlipFaulted source, string key)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The key must not be empty", "key");

            return DeserializeVariable<T>(key, source.Variables);
        }

        static T DeserializeVariable<T>(string key, IDictionary<string, object> dictionary)
        {
            object obj;
            if (!dictionary.TryGetValue(key, out obj))
                throw new KeyNotFoundException("The variable was not present: " + key);

            var token = obj as JToken;
            if (token == null)
            {
                if (typeof(T).IsValueType || typeof(T) == typeof(string))
                    return (T)obj;

                token = new JObject();
            }

            if (token.Type == JTokenType.Null)
                token = new JObject();

            using (var jsonReader = new JTokenReader(token))
                return (T)SerializerCache.Deserializer.Deserialize(jsonReader, typeof(T));
        }
    }
}