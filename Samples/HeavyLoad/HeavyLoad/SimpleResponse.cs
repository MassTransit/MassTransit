namespace HeavyLoad
{
	using System;
	using MassTransit.ServiceBus;

	[Serializable]
	public class SimpleResponse : IMessage
	{
		private readonly int[] _values;

		public SimpleResponse()
		{
			_values = new int[64];
		}

		public int[] Values
		{
			get { return _values; }
		}
	}
}