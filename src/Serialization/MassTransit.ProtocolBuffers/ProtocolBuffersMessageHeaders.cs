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
namespace MassTransit.ProtocolBuffers
{
    using System;
    using System.Collections.Generic;
    using Util;


    public class ProtocolBuffersMessageHeaders :
        Headers
    {
        readonly IObjectTypeDeserializer _deserializer;
        readonly IDictionary<string, object> _headers;

        public ProtocolBuffersMessageHeaders(IObjectTypeDeserializer deserializer, IDictionary<string, object> headers)
        {
            _deserializer = deserializer;
            _headers = headers ?? new Dictionary<string, object>();
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _headers;
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _headers.TryGetValue(key, out value);
        }

        T Headers.Get<T>(string key, T defaultValue)
        {
            return _deserializer.Deserialize(_headers, key, defaultValue);
        }

        public T? Get<T>(string key, T? defaultValue = null) where T : struct
        {
            return _deserializer.Deserialize(_headers, key, defaultValue);
        }
    }
}