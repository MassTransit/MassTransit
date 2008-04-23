namespace MassTransit.Patterns.Fabric
{
	public interface Dispatches<TMessage> :
		Consumes<TMessage>,
		Produces<TMessage>
	{
	}
}