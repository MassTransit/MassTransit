namespace WebRequestReply.Core
{
	using Castle.Facilities.FactorySupport;
	using Castle.Windsor;
	using MassTransit.ServiceBus;
	using MassTransit.WindsorIntegration;

	public class Container : WindsorContainer
	{
		private static readonly Container _container;

		static Container()
		{
			_container = new Container();
		}

		private Container()
			: base("castle.xml")
		{
			LoadMassTransit();

			Resolve<IServiceBus>().Subscribe<RequestMessage>(HandleRequestMessage);
		}

		public static Container Instance
		{
			get { return _container; }
		}

		private void LoadMassTransit()
		{
			AddFacility("factory.support", new FactorySupportFacility());
			AddFacility("masstransit", new MassTransitFacility());
		}

		private static void HandleRequestMessage(IMessageContext<RequestMessage> ctx)
		{
			ResponseMessage response = new ResponseMessage(ctx.Message.CorrelationId, "Request: " + ctx.Message.Text);

			Instance.Resolve<IServiceBus>().Publish(response);
		}
	}
}