namespace MassTransit.Util
{
	using System.Collections.Generic;

	public struct Tuple<TKey, TValue>
	{
		private readonly TKey _key;
		private readonly TValue _value;

		public Tuple(TKey key, TValue value)
		{
			_key = key;
			_value = value;
		}

		public Tuple(KeyValuePair<TKey, TValue> pair)
		{
			_key = pair.Key;
			_value = pair.Value;
		}

		public TValue Value
		{
			get { return _value; }
		}

		public TKey Key
		{
			get { return _key; }
		}
	}
}