namespace MassTransit.Patterns.Fabric
{
	public interface Consumes<TMessage>
	{
		void Consume(TMessage message);
	}
}