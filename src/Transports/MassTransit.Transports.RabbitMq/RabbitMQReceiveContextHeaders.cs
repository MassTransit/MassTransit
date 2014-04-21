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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    ///     Encapsulates the headers included in an IBasicProperties so that they are accessible
    ///     as transport headers on the ReceiveContext
    /// </summary>
    public class RabbitMqReceiveContextHeaders :
        Headers
    {
        readonly RabbitMqBasicConsumeContext _context;

        public RabbitMqReceiveContextHeaders(RabbitMqBasicConsumeContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        T Headers.Get<T>(string key, T defaultValue)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            object value;
            if (!TryGetHeaderValue(key, out value))
                return defaultValue;

            if (value == null)
                return defaultValue;

            if (typeof(T) == typeof(string))
            {
                if (value is string)
                    return (T) value;

                return (T) (object) value.ToString();
            }

            return value as T ?? defaultValue;
        }

        T? Headers.Get<T>(string key, T? defaultValue)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            object value;
            if (!TryGetHeaderValue(key, out value))
                throw new KeyNotFoundException("The header is not present: " + key);

            if (value == null)
                return defaultValue;

            var s = value as string;
            if (s != null)
            {
                if (typeof(T) == typeof(int))
                    return (T) (object) int.Parse(s);
                if (typeof(T) == typeof(decimal))
                    return (T) (object) decimal.Parse(s);
                if (typeof(T) == typeof(long))
                    return (T) (object) long.Parse(s);
                if (typeof(T) == typeof(DateTime))
                    return (T) (object) DateTime.Parse(s);
                if (typeof(T) == typeof(DateTimeOffset))
                    return (T) (object) DateTimeOffset.Parse(s);
                if (typeof(T) == typeof(double))
                    return (T) (object) double.Parse(s);
                if (typeof(T) == typeof(Guid))
                    return (T) (object) Guid.Parse(s);
            }

            return (T) value;
        }

        bool Headers.TryGetHeader(string key, out object value)
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