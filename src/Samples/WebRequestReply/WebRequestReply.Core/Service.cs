namespace WebRequestReply.Core
{
	using MassTransit;

	public class Service :
		Consumes<RequestMessage>.Context
	{
		readonly IServiceBus _bus;

		public Service(IServiceBus bus)
		{
			_bus = bus;
		}

		public void Consume(IConsumeContext<RequestMessage> context)
		{
			context.Respond(new ResponseMessage(context.Message.CorrelationId, "Request: " + context.Message.Text));
		}
	}
}