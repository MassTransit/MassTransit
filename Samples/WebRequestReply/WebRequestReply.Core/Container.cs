namespace WebRequestReply.Core
{
    using MassTransit;
	using MassTransit.WindsorIntegration;

	public class Container :
		DefaultMassTransitContainer
	{
		private static readonly Container _container;

		static Container()
		{
			_container = new Container();
		}

		private Container()
			: base("castle.xml")
		{
			Resolve<IServiceBus>().Subscribe<RequestMessage>(HandleRequestMessage);
		}

		public static Container Instance
		{
			get { return _container; }
		}

		private static void HandleRequestMessage(RequestMessage message)
		{
			ResponseMessage response = new ResponseMessage(message.CorrelationId, "Request: " + message.Text);

			Instance.Resolve<IServiceBus>().Publish(response);
		}
	}
}