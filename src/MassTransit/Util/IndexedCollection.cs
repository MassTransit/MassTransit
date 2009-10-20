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
namespace MassTransit.Util
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Reflection;
	using Magnum.ObjectExtensions;

	public class IndexedCollection<T> :
		Collection<T>
	{
		private readonly Dictionary<string, Dictionary<int, List<T>>> _indices = new Dictionary<string, Dictionary<int, List<T>>>();

		public IndexedCollection()
		{
			BuildIndices();
		}

		public IndexedCollection(IEnumerable<T> list)
		{
			BuildIndices();
			list.Each(Add);
		}

		public new void Add(T newItem)
		{
			_indices.Keys.Each(key =>
				{
					PropertyInfo property = typeof (T).GetProperty(key);
					if (property != null)
					{
						int hashCode = property.GetValue(newItem, null).GetHashCode();
						Dictionary<int, List<T>> index = _indices[key];
						if (index.ContainsKey(hashCode))
							index[hashCode].Add(newItem);
						else
						{
							var newList = new List<T>(1) {newItem};
							index.Add(hashCode, newList);
						}
					}
				});

			base.Add(newItem);
		}

		public new void Remove(T item)
		{
			_indices.Keys.Each(key =>
			{
				PropertyInfo property = typeof(T).GetProperty(key);
				if (property != null)
				{
					int hashCode = property.GetValue(item, null).GetHashCode();
					Dictionary<int, List<T>> index = _indices[key];
					if (index.ContainsKey(hashCode))
						index[hashCode].Remove(item);
				}
			});

			base.Remove(item);
		}

		public bool PropertyHasIndex(string propertyName)
		{
			return _indices.ContainsKey(propertyName);
		}

		public Dictionary<int, List<T>> GetIndexByProperty(string propertyName)
		{
			Dictionary<int, List<T>> result;
			if (_indices.TryGetValue(propertyName, out result))
				return result;

			return null;
		}

		private void BuildIndices()
		{
			typeof (T).GetProperties()
				.Where(x => x.GetAttribute<IndexedAttribute>() != null)
				.Each(property => { _indices.Add(property.Name, new Dictionary<int, List<T>>()); });
		}
	}
}