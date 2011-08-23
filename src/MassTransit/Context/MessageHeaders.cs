// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class MessageHeaders :
        IMessageHeaders
    {
        readonly IDictionary<string, string> _headers;

        public MessageHeaders()
        {
            _headers = new Dictionary<string, string>();
        }

        public MessageHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            _headers = headers.ToDictionary(x => x.Key, x => x.Value);
        }

        public string this[string key]
        {
            get
            {
                string value;
                if (_headers.TryGetValue(key, out value))
                    return value;

                return null;
            }
            set
            {
                if (value == null)
                {
                    _headers.Remove(key);
                    return;
                }

                _headers[key] = value;
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _headers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}