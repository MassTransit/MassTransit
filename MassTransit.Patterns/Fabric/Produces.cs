namespace MassTransit.Patterns.Fabric
{
	public interface Produces<TMessage>
	{
		void Attach(Consumes<TMessage> consumer);
	}
}