// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using Apache.NMS;
    using Util;


    public class ActiveMqHeaderAdapter :
        SendHeaders
    {
        readonly IPrimitiveMap _properties;

        public ActiveMqHeaderAdapter(IPrimitiveMap properties)
        {
            _properties = properties;
        }

        public void Set(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _properties.Remove(key);
            else
                _properties[key] = value;
        }

        public void Set(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _properties.Remove(key);
            else
                _properties[key] = value;
        }

        public bool TryGetHeader(string key, out object value)
        {
            var found = _properties.Contains(key);
            if (found)
            {
                value = _properties[key];
                return true;
            }

            value = null;
            return false;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            foreach (string key in _properties.Keys)
            {
                var value = _properties[key];

                yield return new KeyValuePair<string, object>(key, value);
            }
        }

        T Headers.Get<T>(string key, T defaultValue)
        {
            if (TryGetHeader(key, out var value))
                return ObjectTypeDeserializer.Deserialize(value, defaultValue);

            return defaultValue;
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : struct
        {
            if (TryGetHeader(key, out var value))
                return ObjectTypeDeserializer.Deserialize(value, defaultValue);

            return defaultValue;
        }
    }
}
