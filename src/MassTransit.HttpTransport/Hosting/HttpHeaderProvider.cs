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
namespace MassTransit.HttpTransport.Hosting
{
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Context;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;


    public class HttpHeaderProvider :
        IHeaderProvider
    {
        readonly IHeaderDictionary _headers;

        public HttpHeaderProvider(IHeaderDictionary headers)
        {
            _headers = headers;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _headers.Keys.SelectMany(k => _headers[k].Select(v => new KeyValuePair<string, object>(k, v)));
        }

        public bool TryGetHeader(string key, out object value)
        {
            var result = _headers.TryGetValue(key, out StringValues values);
            value = values.FirstOrDefault();
            return result;
        }
    }
}