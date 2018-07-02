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
namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Collections.Generic;
    using Context;


    public class ServiceBusHeaderProvider :
        IHeaderProvider
    {
        readonly ServiceBusReceiveContext _context;

        public ServiceBusHeaderProvider(ServiceBusReceiveContext context)
        {
            _context = context;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            yield return new KeyValuePair<string, object>(nameof(_context.MessageId), _context.MessageId);
            yield return new KeyValuePair<string, object>(nameof(_context.CorrelationId), _context.CorrelationId);

            if (_context.Properties != null)
            {
                foreach (KeyValuePair<string, object> header in _context.Properties)
                    yield return header;
            }
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (_context.Properties == null)
            {
                value = null;
                return false;
            }

            if (nameof(_context.MessageId).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.MessageId;
                return true;
            }

            if (nameof(_context.CorrelationId).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = _context.CorrelationId;
                return true;
            }

            return _context.Properties.TryGetValue(key, out value);
        }
    }
}