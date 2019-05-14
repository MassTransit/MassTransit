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
namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using RabbitMQ.Client;
    using Util;


    public class RabbitMqSendHeaders :
        SendHeaders
    {
        readonly IBasicProperties _basicProperties;

        public RabbitMqSendHeaders(IBasicProperties basicProperties)
        {
            _basicProperties = basicProperties;
        }

        public void Set(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_basicProperties.Headers == null)
                _basicProperties.Headers = new Dictionary<string, object>();

            if (value == null)
                _basicProperties.Headers.Remove(key);
            else
                _basicProperties.Headers[key] = value;
        }

        public void Set(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_basicProperties.Headers == null)
                _basicProperties.Headers = new Dictionary<string, object>();

            if (value == null)
                _basicProperties.Headers.Remove(key);
            else
                _basicProperties.Headers[key] = value;
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (_basicProperties.Headers == null)
            {
                value = null;
                return false;
            }

            bool found = _basicProperties.Headers.TryGetValue(key, out value);
            if (found)
            {
                if (value is byte[])
                    value = Encoding.UTF8.GetString((byte[])value);
            }

            return found;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            if (_basicProperties.IsHeadersPresent())
                return _basicProperties.Headers;

            return Enumerable.Empty<KeyValuePair<string, object>>();
        }

        T MassTransit.Headers.Get<T>(string key, T defaultValue)
        {
            object value;
            if (TryGetHeader(key, out value))
                return ObjectTypeDeserializer.Deserialize(value, defaultValue);

            return defaultValue;
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : struct
        {
            object value;
            if (TryGetHeader(key, out value))
                return ObjectTypeDeserializer.Deserialize(value, defaultValue);

            return defaultValue;
        }
    }
}
