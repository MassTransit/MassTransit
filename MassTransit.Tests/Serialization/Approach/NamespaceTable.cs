namespace MassTransit.Tests.Serialization.Approach
{
	using System;
	using System.Collections.Generic;
	using Magnum.ObjectExtensions;

	public class NamespaceTable
	{
		private Dictionary<string, string> _mapNamespaceToPrefix = new Dictionary<string, string>();
		private HashSet<string> _prefixes = new HashSet<string>();
		private int _counter = 1;


		public void Each(Action<string,string> action)
		{
			foreach (KeyValuePair<string, string> pair in _mapNamespaceToPrefix)
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

		private string GeneratePrefix(string localName, string ns)
		{
			string prefix = localName.IsNullOrEmpty() ? "o" : char.ToLower(localName[0]).ToString();

			if (_prefixes.Contains(prefix))
				prefix += _counter++;

			return prefix;
		}
	}
}