namespace MassTransit.Patterns.Fabric
{
	using ServiceBus;

	public interface ITransformer<TIn, TOut> :
		IConsume<TIn>,
		IProduce<TOut>
		where TIn : IMessage
		where TOut : IMessage
	{
	}
}