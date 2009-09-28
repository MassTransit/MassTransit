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
namespace MassTransit.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class IdempotentList<T> :
		IEnumerable<T>
	{
		private readonly IList<T> _list;

		public IdempotentList()
		{
			_list = new List<T>();
		}

		public int Count
		{
			get { return _list.Count; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public void Add(T item)
		{
			if (!_list.Contains(item))
			{
				_list.Add(item);
			}
		}

		public void Remove(T item)
		{
			if (_list.Contains(item))
			{
				_list.Remove(item);
			}
		}

		public void Clear()
		{
			_list.Clear();
		}
	}
}