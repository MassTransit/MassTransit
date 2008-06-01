namespace MassTransit.ServiceBus
{
	public class Consumes<TMessage> where TMessage : class
	{
		public interface All
		{
			void Consume(TMessage message);
		}

		public interface For<TCorrelationId> : All, CorrelatedBy<TCorrelationId>
		{
		}

		public interface Selected : All
		{
			bool Accept(TMessage message);
		}
	}
}