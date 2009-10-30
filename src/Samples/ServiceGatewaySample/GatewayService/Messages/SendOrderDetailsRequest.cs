using System;

namespace GatewayService.Messages
{
	public class SendOrderDetailsRequest
	{
		public Guid RequestId { get; set; }
		public string OrderId { get; set; }
		public string CustomerId { get; set; }
	}
}