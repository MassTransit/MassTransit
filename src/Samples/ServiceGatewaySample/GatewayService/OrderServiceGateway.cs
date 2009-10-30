using MassTransit;

namespace GatewayService
{
	public class OrderServiceGateway
	{
		private readonly IServiceBus _bus;

		public OrderServiceGateway(IServiceBus bus)
		{
			_bus = bus;
		}

		public void Start()
		{
			_bus.Subscribe<OrderDetailsWebServiceProxy>();
		}

		public void Stop()
		{
			// no need to unsubscribe, we're a service
		}
	}
}