namespace MassTransit.ServiceBus
{
	using System;

	public interface IServiceBusRequest : IAsyncResult, IDisposable
	{
		/// <summary>
		/// Called by the consumer class to indicate that the operation is complete
		/// </summary>
		void Complete();

		/// <summary>
		/// Cancels a pending asynchronous request. Any messages received after the request is cancelled are ignored.
		/// </summary>
		void Cancel();
	}
}