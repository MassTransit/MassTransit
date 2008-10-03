namespace CodeCamp.Core
{
    using Castle.MicroKernel;
    using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.MSMQ;
	using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.WindsorIntegration;

    public static class DomainContext
	{
		private static readonly IEndpoint _endpoint;
		private static readonly IServiceBus _serviceBus;
		private static readonly Repository<User> _userRepository;

		static DomainContext()
		{
			_userRepository = new Repository<User>(new [] {new User("joe", "password"), new User("david", "password"),});

			_endpoint = new MsmqEndpoint("msmq://localhost/mt_client");

			ISubscriptionCache cache = new DistributedSubscriptionCache();

            IObjectBuilder obj = new WindsorObjectBuilder(new DefaultKernel());

			_serviceBus = new ServiceBus(_endpoint, obj, cache);
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