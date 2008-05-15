namespace MassTransit.ServiceBus
{
	public class Consumes<TMessage> where TMessage : class
	{
		public interface Any
		{
			void Consume(TMessage message);
		}

		public interface For<TCorrelationId> : Any, CorrelatedBy<TCorrelationId>
		{
		}

		public interface Selected : Any
		{
			bool Accept(TMessage message);
		}
	}
}