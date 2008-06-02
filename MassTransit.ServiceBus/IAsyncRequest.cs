namespace MassTransit.ServiceBus
{
	using System;

	public interface IAsyncRequest : IAsyncResult
	{
		void Complete();
		void Complete(bool synchronously);
		void Cancel();
	}
}