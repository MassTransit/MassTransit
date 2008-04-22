namespace MassTransit.ServiceBus
{
	using Internal;

	public static class Respond
	{
		public static void With(params IMessage[] messages)
		{
			IEndpoint replyEndpoint = ServiceBusContext.Envelope.ReturnEndpoint;

			IEnvelope envelope = new Envelope(ServiceBusContext.Bus.Endpoint, messages);

			envelope.CorrelationId = ServiceBusContext.Envelope.Id;

			replyEndpoint.Sender.Send(envelope);
		}
		
	}
}