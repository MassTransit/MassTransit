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
namespace MassTransit.Util
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Magnum.Reflection;
	using Saga;

	public interface IndexedSagaProperty<TSaga>
		where TSaga : class, ISaga
	{
		void Add(TSaga newItem);
		void Remove(TSaga item);
		
		TSaga this[object key] { get; }

		IEnumerable<TSaga> Where(Func<TSaga, bool> filter);
		IEnumerable<TSaga> Where(object key, Func<TSaga, bool> filter);

		IEnumerable<TResult> Select<TResult>(Func<TSaga, TResult> transformer);
	}

	public class IndexedSagaProperty<TSaga, TProperty> :
		IndexedSagaProperty<TSaga>
		where TSaga : class, ISaga
	{
		readonly FastProperty<TSaga, TProperty> _property;
		readonly IDictionary<TProperty, HashSet<TSaga>> _values;

		public IndexedSagaProperty(PropertyInfo propertyInfo)
		{
			_values = new Dictionary<TProperty, HashSet<TSaga>>();
			_property = new FastProperty<TSaga, TProperty>(propertyInfo, BindingFlags.NonPublic);
		}

		public TSaga this[object key]
		{
			get
			{
				var keyValue = (TProperty) key;

				HashSet<TSaga> result;
				if (_values.TryGetValue(keyValue, out result))
					return result.Single();

				return null;
			}
		}

		public void Add(TSaga newItem)
		{
			TProperty key = _property.Get(newItem);

			HashSet<TSaga> hashSet;
			if (!_values.TryGetValue(key, out hashSet))
			{
				hashSet = new HashSet<TSaga>();
				_values.Add(key, hashSet);
			}

			hashSet.Add(newItem);
		}

		public void Remove(TSaga item)
		{
			TProperty key = _property.Get(item);

			HashSet<TSaga> hashSet;
			if (!_values.TryGetValue(key, out hashSet))
				return;

			hashSet.Remove(item);
		}

		public IEnumerable<TSaga> Where(Func<TSaga, bool> filter)
		{
			return _values.Values.SelectMany(x => x).Where(filter);
		}

		public IEnumerable<TSaga> Where(object key, Func<TSaga, bool> filter)
		{
			var keyValue = (TProperty) key;

			HashSet<TSaga> resultSet;
			if (_values.TryGetValue(keyValue, out resultSet))
			{
				return resultSet.Where(filter);
			}

			return Enumerable.Empty<TSaga>();
		}

		public IEnumerable<TResult> Select<TResult>(Func<TSaga, TResult> transformer)
		{
			return _values.Values.SelectMany(x => x).Select(transformer);
		}
	}
}