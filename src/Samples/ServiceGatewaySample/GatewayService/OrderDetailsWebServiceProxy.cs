using GatewayService.Interfaces;
using Magnum;
using Magnum.DateTimeExtensions;
using MassTransit;

namespace GatewayService
{
	public class OrderDetailsWebServiceProxy :
		Consumes<RetrieveOrderDetails>.All
	{
		public void Consume(RetrieveOrderDetails request)
		{
			// simulate a call to the external service

			ThreadUtil.Sleep(1.Seconds());

			var details = new OrderDetailsResponse
			              	{
			              		OrderId = request.OrderId,
			              		CustomerId = request.CustomerId,
			              		Created = (-1).Days().FromUtcNow(),
			              		Status = OrderStatus.InProcess,
			              	};

			CurrentMessage.Respond(details, x => x.ExpiresAt(5.Minutes().FromNow()));
		}
	}
}