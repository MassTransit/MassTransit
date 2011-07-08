// Copyright 2007-2011 The Apache Software Foundation.
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
	using System.Collections;
	using System.Collections.Generic;

	public class IdempotentHashtable<K, V> :
		IEnumerable<V>
	{
		private readonly IDictionary<K, V> _cache = new Dictionary<K, V>();

		public V this[K key]
		{
			get { return _cache[key]; }
			set { Add(key, value); }
		}

		public int Count
		{
			get { return _cache.Count; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<V> GetEnumerator()
		{
			return _cache.Values.GetEnumerator();
		}

		public void Add(K key, V value)
		{
			if (!_cache.ContainsKey(key))
			{
				_cache.Add(key, value);
			}
		}

		public void Remove(K key)
		{
			if (_cache.ContainsKey(key))
			{
				_cache.Remove(key);
			}
		}

		public bool Contains(K key)
		{
			return _cache.ContainsKey(key);
		}
	}
}