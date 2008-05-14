namespace MassTransit.ServiceBus
{
	public interface CorrelatedBy<TKey>
	{
		TKey CorrelationId { get; }
	}


	public class Consumes<TMessage> where TMessage : class
	{
		public interface Any
		{
			void Consume(TMessage message);
		}

		public interface For<TKey> : Any, CorrelatedBy<TKey>
		{
		}
	}
}