using System;

namespace GatewayService.Interfaces
{
	/// <summary>
	/// Produced when the details of an order are received
	/// </summary>
	public interface OrderDetailsReceived
	{
		string OrderId { get; }
		string CustomerId { get; }
		DateTime Created { get; }
		OrderStatus Status { get; }
	}
}