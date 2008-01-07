using System;

namespace MassTransit.ServiceBus.Tests
{
	[Serializable]
	public class PoisonMessage : IMessage
	{
		public string ThrowException()
		{
			throw new Exception("POISON"); 
		}
	}
}