namespace MassTransit.Patterns.Fabric
{
	public class RespondsTo<TRequest>
	{
		#region Nested type: With

		public interface With<TResponse> : Consumes<TRequest>, Produces<TResponse>
		{
		}

		#endregion
	}
}