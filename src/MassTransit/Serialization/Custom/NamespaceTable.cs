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
namespace MassTransit.Serialization.Custom
{
	using System;
	using System.Collections.Generic;

	public class NamespaceTable
	{
		readonly Dictionary<string, string> _mapNamespaceToPrefix = new Dictionary<string, string>();
		readonly HashSet<string> _prefixes = new HashSet<string>();
		int _counter = 1;


		public void Each(Action<string, string> action)
		{
			foreach (var pair in _mapNamespaceToPrefix)
			{
				action(pair.Key, pair.Value);
			}
		}

		public string GetPrefix(string localName, string ns)
		{
			string result;
			if (_mapNamespaceToPrefix.TryGetValue(ns, out result))
				return result;

			result = GeneratePrefix(localName, ns);

			_prefixes.Add(result);
			_mapNamespaceToPrefix.Add(ns, result);

			return result;
		}

		string GeneratePrefix(string localName, string ns)
		{
			string prefix = string.IsNullOrEmpty(localName) ? "o" : char.ToLower(localName[0]).ToString();

			if (_prefixes.Contains(prefix))
				prefix += _counter++;

			return prefix;
		}
	}
}