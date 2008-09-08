// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Infrastructure
{
	using System.Collections.Generic;
	using System.Linq;

	public class RepositoryCache<T, K> :
		RepositoryBase<T, K>
		where T : IAggregateRoot<K>
	{
		private readonly Dictionary<K, T> _storage;

		public RepositoryCache()
		{
			_storage = new Dictionary<K, T>();
		}

		protected override IQueryable<T> RepositoryQuery
		{
			get { return _storage.Values.AsQueryable(); }
		}

		public override T Get(K id)
		{
			return _storage.ContainsKey(id) ? _storage[id] : default(T);
		}

		public override void Save(T item)
		{
			if (_storage.ContainsKey(item.Id))
				_storage[item.Id] = item;
			else
				_storage.Add(item.Id, item);
		}

		public override void Delete(T item)
		{
			if (_storage.ContainsKey(item.Id))
				_storage.Remove(item.Id);
		}
	}
}