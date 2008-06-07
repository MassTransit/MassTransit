namespace MassTransit.ServiceBus.Internal
{
	public class ComponentDispatcher<TComponent, TMessage> :
		Consumes<TMessage>.Selected 
		where TMessage : class
		where TComponent : class
	{
		private readonly IObjectBuilder _builder;

		public ComponentDispatcher(IObjectBuilder builder)
		{
			_builder = builder;
		}

		public bool Accept(TMessage message)
		{
			return true;
		}

		public void Consume(TMessage message)
		{
			Consumes<TMessage>.All consumer = _builder.Build<Consumes<TMessage>.All>(typeof (TComponent));

			consumer.Consume(message);

			_builder.Release(consumer);
		}
	}
}