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

	public class LambdaEqualityComparer<T> :
		IEqualityComparer<T>
	{
		readonly Func<T, T, bool> _comparer;
		readonly Func<T, int> _hash;

		public LambdaEqualityComparer(Func<T, T, bool> comparer)
			: this(comparer, o => 0)
		{
		}

		public LambdaEqualityComparer(Func<T, T, bool> comparer, Func<T, int> hash)
		{
			if (comparer == null)
				throw new ArgumentNullException(nameof(comparer));
			if (hash == null)
				throw new ArgumentNullException(nameof(hash));

			_comparer = comparer;
			_hash = hash;
		}

		public bool Equals(T x, T y)
		{
			return _comparer(x, y);
		}

		public int GetHashCode(T obj)
		{
			return _hash(obj);
		}
	}

	public static class LambdaEqualityComparerExtensions
	{
		public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> comparer)
		{
			return source.Distinct(new LambdaEqualityComparer<T>(comparer));
		}

		public static IEnumerable<T> Except<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> comparer)
		{
			return first.Except(second, new LambdaEqualityComparer<T>(comparer));
		}
	}
}