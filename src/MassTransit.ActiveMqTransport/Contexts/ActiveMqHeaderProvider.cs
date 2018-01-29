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
namespace MassTransit.ActiveMqTransport.Contexts
{
    using System.Collections.Generic;
    using Apache.NMS;
    using Context;


    public class ActiveMqHeaderProvider :
        IHeaderProvider
    {
        readonly ActiveMqHeaderAdapter _adapter;

        public ActiveMqHeaderProvider(IPrimitiveMap properties)
        {
            _adapter = new ActiveMqHeaderAdapter(properties);
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            return _adapter.GetAll();
        }

        public bool TryGetHeader(string key, out object value)
        {
            return _adapter.TryGetHeader(key, out value);
        }
    }
}