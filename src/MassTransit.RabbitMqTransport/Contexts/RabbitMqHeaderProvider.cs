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
    using System.Text;
    using Context;


    public class RabbitMqHeaderProvider :
        IHeaderProvider
    {
        readonly RabbitMqBasicConsumeContext _context;

        public RabbitMqHeaderProvider(RabbitMqBasicConsumeContext context)
        {
            _context = context;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            yield return new KeyValuePair<string, object>(RabbitMqHeaders.Exchange, _context.Exchange);
            yield return new KeyValuePair<string, object>(RabbitMqHeaders.RoutingKey, _context.RoutingKey);
            yield return new KeyValuePair<string, object>(RabbitMqHeaders.DeliveryTag, _context.DeliveryTag);
            yield return new KeyValuePair<string, object>(RabbitMqHeaders.ConsumerTag, _context.ConsumerTag);

            if (_context.Properties.IsHeadersPresent())
            {
                foreach (var header in _context.Properties.Headers)
                    yield return header;
            }
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (!_context.Properties.IsHeadersPresent())
            {
                value = null;
                return false;
            }

            if (_context.Properties.Headers.TryGetValue(key, out value))
            {
                if (value is byte[])
                {
                    value = Encoding.UTF8.GetString((byte[])value);
                    return !string.IsNullOrWhiteSpace((string)value);
                }
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