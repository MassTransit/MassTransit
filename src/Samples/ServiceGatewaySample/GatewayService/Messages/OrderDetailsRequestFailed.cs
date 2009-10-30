using System;

namespace GatewayService.Messages
{
	public class OrderDetailsRequestFailed
	{
		public Guid RequestId { get; set; }
		public string Message { get; set; }
	}
}