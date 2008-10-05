using CodeCamp.Domain;

namespace CodeCamp.Core
{
    using Castle.MicroKernel;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Configuration;
    using MassTransit.ServiceBus.MSMQ;
	using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.WindsorIntegration;

    public static class DomainContext
	{
		private static readonly IEndpoint _endpoint;
		private static readonly IServiceBus _serviceBus;
		private static readonly IRepository<User> _userRepository;

		static DomainContext()
		{
			//_userRepository = new Repository<User>(new [] {new User("joe", "password"), new User("david", "password"),});

		    BusBuilder.SetObjectBuilder(new WindsorObjectBuilder(new DefaultKernel()));
            _serviceBus = BusBuilder.WithName("bill")
		        .ListensOn("msmq://localhost/mt_client")
                .SharesSubscriptionsVia<DistributedSubscriptionCache>("tcpip://127.0.0.1:11211")
		        .CommunicatesOver<MsmqEndpoint>()
		        .Validate()
		        .Build();
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