namespace GatewayService.Tests
{
	using Interfaces;

	public class RetrieveOrderDetailsRequest : 
		RetrieveOrderDetails
	{
		public RetrieveOrderDetailsRequest(string customerId, string orderId)
		{
			CustomerId = customerId;
			OrderId = orderId;
		}

		public string OrderId { get; private set; }
		public string CustomerId { get; private set; }
	}
}