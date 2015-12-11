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
namespace MassTransit.Util
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    public class LazyConcurrentDictionary<TKey, TValue>
    {
        public delegate Task<TValue> ValueFactory(TKey key);


        readonly ValueFactory _valueFactory;
        readonly ConcurrentDictionary<TKey, Lazy<Task<TValue>>> _values;

        public LazyConcurrentDictionary(ValueFactory valueFactory)
        {
            _valueFactory = valueFactory;
            _values = new ConcurrentDictionary<TKey, Lazy<Task<TValue>>>();
        }

        public LazyConcurrentDictionary(ValueFactory valueFactory, IEqualityComparer<TKey> comparer)
        {
            _valueFactory = valueFactory;
            _values = new ConcurrentDictionary<TKey, Lazy<Task<TValue>>>(comparer);
        }

        public async Task<TValue> Get(TKey key)
        {
            Lazy<Task<TValue>> result = _values.GetOrAdd(key, x => new Lazy<Task<TValue>>(() => _valueFactory(x), LazyThreadSafetyMode.PublicationOnly));

            return await result.Value;
        }
    }
}