namespace MassTransit.SubscriptionConnectors
{
	using System;

	internal class ConsumptionPair
	{
		public ConsumptionPair(Type interfaceType, Type messageType)
		{
			InterfaceType = interfaceType;
			MessageType = messageType;
		}

		public Type InterfaceType { get; private set; }
		public Type MessageType { get; private set; }
	}
}