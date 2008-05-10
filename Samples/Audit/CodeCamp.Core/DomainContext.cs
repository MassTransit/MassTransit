namespace CodeCamp.Core
{
	using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.MSMQ;
	using MassTransit.ServiceBus.Subscriptions;

	public static class DomainContext
	{
		private static readonly SubscriptionClient _client;
		private static readonly IEndpoint _endpoint;
		private static readonly IServiceBus _serviceBus;
		private static readonly IEndpoint _subscriptionEndpoint;
		private static readonly Repository<User> _userRepository;

		static DomainContext()
		{
			_userRepository = new Repository<User>(new User[] {new User("joe", "password"), new User("david", "password"),});

			_endpoint = new MsmqEndpoint("msmq://localhost/test_servicebus");
			_subscriptionEndpoint = new MsmqEndpoint("msmq://localhost/test_subscriptions");

			ISubscriptionCache cache = new LocalSubscriptionCache();

			_serviceBus = new ServiceBus(_endpoint, cache);

			_client = new SubscriptionClient(_serviceBus, cache, _subscriptionEndpoint);
			_client.Start();
		}

		public static IRepository<User> UserRepository
		{
			get { return _userRepository; }
		}

		public static IServiceBus ServiceBus
		{
			get { return _serviceBus; }
		}

		public static void Initialize()
		{
		}
	}
}