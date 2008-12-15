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
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Magnum.Common.Threading;

	public class ReaderWriterLockedDictionary<TKey, TValue>
	{
		private readonly ReaderWriterLockedObject<Dictionary<TKey, TValue>> _collection;

		public ReaderWriterLockedDictionary()
		{
			_collection = new ReaderWriterLockedObject<Dictionary<TKey, TValue>>(new Dictionary<TKey, TValue>());
		}

		public IEnumerable<TValue> Values
		{
			get { return _collection.ReadLock(x => x.Values); }
		}

		public TValue Retrieve(TKey key, Func<TValue> valueProvider)
		{
			TValue result = default(TValue);

			return _collection.ReadLock(x => x.TryGetValue(key, out result)) ? result : _collection.WriteLock(x =>
				{
					if (x.TryGetValue(key, out result))
						return result;

					result = valueProvider();

					x[key] = result;

					return result;
				});
		}

		public void Clear()
		{
			_collection.WriteLock(x => x.Clear());
		}
	}
}