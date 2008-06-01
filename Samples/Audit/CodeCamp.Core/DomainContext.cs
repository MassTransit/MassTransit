namespace CodeCamp.Core
{
	using MassTransit.DistributedSubscriptionCache;
	using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.MSMQ;
	using MassTransit.ServiceBus.Subscriptions;

	public static class DomainContext
	{
		private static readonly IEndpoint _endpoint;
		private static readonly IServiceBus _serviceBus;
		private static readonly Repository<User> _userRepository;

		static DomainContext()
		{
			_userRepository = new Repository<User>(new User[] {new User("joe", "password"), new User("david", "password"),});

			_endpoint = new MsmqEndpoint("msmq://localhost/test_servicebus");

			ISubscriptionCache cache = new DistributedSubscriptionCache();

			_serviceBus = new ServiceBus(_endpoint, cache);
		}

		public static void Publish<T>(T message) where T : class
		{
			_serviceBus.Publish(message);
		}

		public static IRepository<User> UserRepository
		{
			get { return _userRepository; }
		}

		public static void Initialize()
		{
		}
	}
}