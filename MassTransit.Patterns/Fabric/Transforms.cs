namespace MassTransit.Patterns.Fabric
{
	public class Transforms<TIn>
	{
		public interface Into<TOut> : Consumes<TIn>, Produces<TOut>
		{
			
		}
	}
}