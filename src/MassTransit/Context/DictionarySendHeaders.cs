// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using Util;


    public class DictionarySendHeaders :
        SendHeaders
    {
        readonly IDictionary<string, object> _headers;

        public DictionarySendHeaders()
        {
            _headers = new Dictionary<string, object>();
        }

        public DictionarySendHeaders(IDictionary<string, object> dictionary)
        {
            _headers = dictionary;
        }

        void SendHeaders.Set(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _headers.Remove(key);
            else
                _headers[key] = value;
        }

        void SendHeaders.Set(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                _headers.Remove(key);
            else
                _headers[key] = value;
        }

        bool Headers.TryGetHeader(string key, out object value)
        {
            return _headers.TryGetValue(key, out value);
        }

        IEnumerable<KeyValuePair<string, object>> Headers.GetAll()
        {
            return _headers;
        }

        T Headers.Get<T>(string key, T defaultValue)
        {
            return ObjectTypeDeserializer.Deserialize(_headers, key, defaultValue);
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : struct
        {
            return ObjectTypeDeserializer.Deserialize(_headers, key, defaultValue);
        }
    }
}
