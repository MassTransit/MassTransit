using System;

namespace MassTransit.ServiceBus.Tests
{
	[Serializable]
	public class ClientMessage
	{
		private string _name;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}
}