namespace MassTransit.ServiceBus.Internal
{
	public class SelectiveComponentDispatcher<TComponent, TMessage> :
		Consumes<TMessage>.Selected
		where TMessage : class
		where TComponent : class
	{
		private readonly IObjectBuilder _builder;

		public SelectiveComponentDispatcher(IObjectBuilder builder)
		{
			_builder = builder;
		}

		public bool Accept(TMessage message)
		{
			Consumes<TMessage>.Selected consumer = _builder.Build<Consumes<TMessage>.Selected>(typeof (TComponent));

			bool result = consumer.Accept(message);

			_builder.Release(consumer);

			return result;
		}

		public void Consume(TMessage message)
		{
			Consumes<TMessage>.Selected consumer = _builder.Build<Consumes<TMessage>.Selected>(typeof (TComponent));

			consumer.Consume(message);

			_builder.Release(consumer);
		}
	}
}