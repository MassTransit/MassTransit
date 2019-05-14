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
namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using Util;


    /// <summary>
    /// The context headers are sourced from the IContextHeaderProvider, with the use of a Json deserializer
    /// to convert data types to objects as required. If the original headers are Json objects, those headers
    /// are deserialized as well
    /// </summary>
    public class JsonHeaders :
        Headers
    {
        readonly IObjectTypeDeserializer _deserializer;
        readonly IHeaderProvider _provider;

        public JsonHeaders(IObjectTypeDeserializer deserializer, IHeaderProvider provider)
        {
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _provider.GetAll();
        }

        bool Headers.TryGetHeader(string key, out object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _provider.TryGetHeader(key, out value);
        }

        T Headers.Get<T>(string key, T defaultValue)
        {
            return _deserializer.Deserialize(_provider, key, defaultValue);
        }

        public T? Get<T>(string key, T? defaultValue)
            where T : struct
        {
            return _deserializer.Deserialize(_provider, key, defaultValue);
        }
    }
}