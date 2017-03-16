// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Util.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class GreenCache<TValue> :
        ICache<TValue>
        where TValue : class
    {
        readonly IDictionary<string, ICacheIndex<TValue>> _indices;
        readonly INodeTracker<TValue> _nodeTracker;

        /// <summary>
        ///
        /// </summary>
        /// <param name="capacity">The typical maximum capacity of the cache (not a hard limit)</param>
        /// <param name="minAge">The minmum time an item is cached before being eligible for removal</param>
        /// <param name="maxAge">The maximum time an unaccessed item will remain in the cache</param>
        /// <param name="nowProvider">Provides the current time</param>
        public GreenCache(int capacity, TimeSpan minAge, TimeSpan maxAge, CurrentTimeProvider nowProvider)
        {
            _indices = new Dictionary<string, ICacheIndex<TValue>>();

            _nodeTracker = new NodeTracker<TValue>(capacity, minAge, maxAge, nowProvider);
        }

        public CacheStatistics Statistics => _nodeTracker.Statistics;

        public IIndex<TKey, TValue> AddIndex<TKey>(string indexName, KeyProvider<TKey, TValue> keyProvider,
            MissingValueFactory<TKey, TValue> missingValueFactory = null)
        {
            if (_indices.ContainsKey(indexName))
                throw new ArgumentException($"An index with the same name was already added: {indexName}", nameof(indexName));

            var index = new Index<TKey, TValue>(_nodeTracker, keyProvider);

            _indices[indexName] = index;

            return index;
        }

        public IIndex<TKey, TValue> GetIndex<TKey>(string indexName)
        {
            ICacheIndex<TValue> index;
            if (_indices.TryGetValue(indexName, out index) && index is IIndex<TKey, TValue>)
                return (IIndex<TKey, TValue>)index;

            throw new ArgumentException($"An index named {indexName} was not found", nameof(indexName));
        }

        public void Add(TValue value)
        {
            _nodeTracker.Add(value);
        }

        public void Clear()
        {
            _nodeTracker.Clear();
        }

        public IEnumerable<TValue> GetAll()
        {
            return _nodeTracker.GetAll().Select(x => x.Value.Result);
        }
    }
}