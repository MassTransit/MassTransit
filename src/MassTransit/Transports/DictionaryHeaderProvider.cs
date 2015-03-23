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
namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Context;


    /// <summary>
    /// A simple in-memory header collection for use with the in memory transport
    /// </summary>
    public class DictionaryHeaderProvider :
        IHeaderProvider
    {
        readonly IDictionary<string, object> _headers;

        public DictionaryHeaderProvider(IDictionary<string, object> headers)
        {
            _headers = headers;
        }

        public IEnumerable<Tuple<string, object>> GetAll()
        {
            return _headers.Select(x => Tuple.Create(x.Key, x.Value));
        }

        public bool TryGetHeader(string key, out object value)
        {
            return _headers.TryGetValue(key, out value);
        }
    }
}