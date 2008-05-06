namespace WebRequestReply.Core
{
	using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.MSMQ;
	using MassTransit.ServiceBus.Subscriptions;

	public class Container
	{
		private static readonly Container _container;
		private readonly IServiceBus _serviceBus;
		private readonly MsmqEndpoint _serviceEndpoint = @"msmq://localhost/test_servicebus";
		private readonly LocalSubscriptionCache _subscriptionCache = new LocalSubscriptionCache();

		static Container()
		{
			_container = new Container();
		}

		private Container()
		{
			_serviceBus = new ServiceBus(_serviceEndpoint, _subscriptionCache);

			_serviceBus.Subscribe<RequestMessage>(HandleRequestMessage);
		}

		public static Container Instance
		{
			get { return _container; }
		}

		public IServiceBus ServiceBus
		{
			get { return _serviceBus; }
		}

		private static void HandleRequestMessage(IMessageContext<RequestMessage> ctx)
		{
			ResponseMessage response = new ResponseMessage();
			response.Text = "Request: " + ctx.Message.Text;

			ctx.Reply(response);
		}

		~Container()
		{
			_serviceBus.Dispose();
		}
	}
}