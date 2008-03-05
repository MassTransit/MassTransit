namespace MassTransit.ServiceBus.Internal
{
	using System;

	public interface IMessageId : 
		IEquatable<IMessageId>
	{
		bool IsEmpty { get; }
	}
}