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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;


    public class AzureServiceBusReceiveContextHeaders :
        Headers
    {
        readonly AzureServiceBusMessageContext _context;

        public AzureServiceBusReceiveContextHeaders(AzureServiceBusMessageContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        T Headers.Get<T>(string key, T defaultValue)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            T value;
            if (!TryGetHeaderValue(key, out value))
                return defaultValue;

            if (value == default(T))
                return defaultValue;

            if (typeof(T) == typeof(string))
            {
                if (value is string)
                    return value;

                return (T)(object)value.ToString();
            }

            return value;
        }

        T? Headers.Get<T>(string key, T? defaultValue)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            T? value;
            if (!TryGetHeaderValue(key, out value))
                throw new KeyNotFoundException("The header is not present: " + key);

            if (!value.HasValue)
                return defaultValue;

            return value;
        }

        bool Headers.TryGetHeader(string key, out object value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return TryGetHeaderValue(key, out value);
        }

        bool TryGetHeaderValue<T>(string key, out T value)
        {
            if (_context.Properties == null)
            {
                value = default(T);
                return false;
            }

            if (_context.Properties.TryGetValue(key, out value))
                return true;

            return false;
        }
    }
}