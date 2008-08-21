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
namespace MassTransit.Grid.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class FactorLongNumbers :
		IDistributedTask<FactorLongNumber, LongNumberFactored>
	{
		private readonly Dictionary<long, IList<long>> _results = new Dictionary<long, IList<long>>();
		private readonly List<long> _values = new List<long>();
		private Action<FactorLongNumbers> _completed;

		public void DeliverResult(long taskId, LongNumberFactored result)
		{
			_results.Add(taskId, result.Factors);

			if (_results.Count == _values.Count)
				_completed(this);
		}

		IEnumerator<FactorLongNumber> IEnumerable<FactorLongNumber>.GetEnumerator()
		{
			long index = 0;
			foreach (long value in _values)
			{
				yield return new FactorLongNumber(index++, value);
			}
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable<FactorLongNumber>) this).GetEnumerator();
		}

		public void Add(long value)
		{
			_values.Add(value);
		}

		public void WhenComplete(Action<FactorLongNumbers> action)
		{
			_completed = action;
		}
	}
}