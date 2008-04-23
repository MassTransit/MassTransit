namespace MassTransit.Patterns.Fabric
{
	using ServiceBus;

	public class RespondsTo<TRequest> where TRequest : IMessage
	{
		#region Nested type: With

		public interface With<TResponse> : IConsume<TRequest>, IProduce<TResponse> where TResponse : IMessage
		{
		}

		#endregion
	}
}