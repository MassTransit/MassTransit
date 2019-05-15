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
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;


    public class StaticHeaders :
        Headers
    {
        readonly Header[] _headers;

        public StaticHeaders(Header[] headers)
        {
            _headers = headers;
        }

        IEnumerable<KeyValuePair<string, object>> Headers.GetAll()
        {
            return _headers.Select(x => new KeyValuePair<string, object>(x.Name, x.Value));
        }

        T Headers.Get<T>(string key, T defaultValue)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            object obj;
            if (!TryGetHeader(key, out obj))
                return defaultValue;

            var result = obj as T;

            return result ?? defaultValue;
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : struct
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            object obj;
            if (!TryGetHeader(key, out obj))
                return defaultValue;

            if (obj == null)
                return defaultValue;

            if (obj is T)
                return (T)obj;

            return defaultValue;
        }

        public bool TryGetHeader(string key, out object value)
        {
            for (int i = 0; i < _headers.Length; i++)
            {
                if (_headers[i].Name.Equals(key))
                {
                    value = _headers[i].Value;
                    return true;
                }
            }

            value = null;
            return false;
        }
    }
}