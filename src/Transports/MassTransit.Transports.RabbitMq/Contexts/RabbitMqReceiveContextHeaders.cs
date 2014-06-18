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
namespace MassTransit.Transports.RabbitMq.Contexts
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Serialization;


    /// <summary>
    ///     Encapsulates the headers included in an IBasicProperties so that they are accessible
    ///     as transport headers on the ReceiveContext
    /// </summary>
    public class RabbitMqReceiveContextHeaders :
        ContextHeaders
    {
        readonly RabbitMqBasicConsumeContext _context;
        readonly JsonSerializer _deserializer;

        public RabbitMqReceiveContextHeaders(RabbitMqBasicConsumeContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _deserializer = JsonMessageSerializer.Deserializer;
        }

        T ContextHeaders.Get<T>(string key, T defaultValue)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            object obj;
            if (!TryGetHeaderValue(key, out obj))
                return defaultValue;

            if (obj == null)
                return defaultValue;

            var token = obj as JToken ?? new JValue(obj);

            if (token.Type == JTokenType.Null)
                return defaultValue;

            using (JsonReader jsonReader = token.CreateReader())
                return (T)_deserializer.Deserialize(jsonReader, typeof(T)) ?? defaultValue;
        }

        T? ContextHeaders.Get<T>(string key, T? defaultValue)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            object obj;
            if (!TryGetHeaderValue(key, out obj))
                throw new KeyNotFoundException("The header is not present: " + key);

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

        bool ContextHeaders.TryGetHeader(string key, out object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return TryGetHeaderValue(key, out value);
        }

        bool TryGetHeaderValue(string key, out object value)
        {
            if (!_context.Properties.IsHeadersPresent())
            {
                value = null;
                return false;
            }

            if (!_context.Properties.Headers.TryGetValue(key, out value))
            {
                return true;
            }

            if (RabbitMqHeaders.Exchange.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.Exchange;
                return true;
            }

            if (RabbitMqHeaders.RoutingKey.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.RoutingKey;
                return true;
            }

            if (RabbitMqHeaders.DeliveryTag.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.DeliveryTag;
                return true;
            }

            if (RabbitMqHeaders.ConsumerTag.Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.ConsumerTag;
                return true;
            }

            return false;
        }
    }
}