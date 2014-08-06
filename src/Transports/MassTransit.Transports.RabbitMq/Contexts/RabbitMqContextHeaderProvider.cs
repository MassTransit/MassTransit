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
    using Context;


    public class RabbitMqContextHeaderProvider :
        IContextHeaderProvider
    {
        readonly RabbitMqBasicConsumeContext _context;

        public RabbitMqContextHeaderProvider(RabbitMqBasicConsumeContext context)
        {
            _context = context;
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (!_context.Properties.IsHeadersPresent())
            {
                value = null;
                return false;
            }

            if (!_context.Properties.Headers.TryGetValue(key, out value))
                return true;

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