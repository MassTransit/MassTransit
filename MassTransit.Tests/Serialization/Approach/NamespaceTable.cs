namespace MassTransit.Tests.Serialization.Approach
{
	using System;
	using System.Collections.Generic;
	using Magnum.ObjectExtensions;

	public class NamespaceTable
	{
		private Dictionary<string, string> _mapNamespaceToPrefix = new Dictionary<string, string>();

		private HashSet<string> _prefixes = new HashSet<string>();


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
			if (localName.IsNullOrEmpty())
				localName = "x";

			string tag = (localName.IsNullOrEmpty() ? "x" : localName.Substring(0, 1)).ToLowerInvariant();
			string prefix = tag;

			int index = 0;
			while(_prefixes.Contains(prefix))
			{
				index++;
				prefix = tag + index;
			}

			return prefix;
		}
	}
}