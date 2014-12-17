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
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Util;


    public class JsonMessageTypeConverter :
        IMessageTypeConverter
    {
        readonly IDictionary<Type, object> _mapped;
        readonly JsonSerializer _serializer;
        readonly string[] _supportedTypes;
        readonly JToken _token;

        public JsonMessageTypeConverter(JsonSerializer serializer, JToken token, IEnumerable<string> supportedTypes)
        {
            _token = token ?? new JObject();
            _supportedTypes = supportedTypes.ToArray();
            _serializer = serializer;
            _mapped = new Dictionary<Type, object>();
        }

        public bool Contains(Type messageType)
        {
            object existing;
            if (_mapped.TryGetValue(messageType, out existing))
                return existing != null;

            string typeUrn = new MessageUrn(messageType).ToString();

            if (_supportedTypes.Any(typeUrn.Equals))
                return true;

            return false;
        }

        public bool TryConvert<T>(out T message)
            where T : class
        {
            if (typeof(T) == typeof(JToken))
            {
                message = _token as T;
                return true;
            }

            object existing;
            if (_mapped.TryGetValue(typeof(T), out existing))
            {
                message = (T)existing;
                return message != null;
            }

            string typeUrn = new MessageUrn(typeof(T)).ToString();

            if (_supportedTypes.Any(typeUrn.Equals))
            {
                object obj;
                Type deserializeType = typeof(T);
                if (deserializeType.IsInterface && deserializeType.IsAllowedMessageType())
                    deserializeType = TypeMetadataCache.GetImplementationType(deserializeType);

                using (var jsonReader = new JTokenReader(_token))
                {
                    obj = _serializer.Deserialize(jsonReader, deserializeType);
                }

                _mapped[typeof(T)] = obj;

                message = (T)obj;
                return true;
            }

            _mapped[typeof(T)] = null;

            message = null;
            return false;
        }
    }
}