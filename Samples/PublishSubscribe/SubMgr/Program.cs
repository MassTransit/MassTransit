namespace SubMgr
{
    internal class Program
    {
		//private static readonly IMessageQueueEndpoint _endpoint = new MessageQueueEndpoint("msmq://localhost/test_subscriptions");
		//private static IServiceBus _bus;
		//private static ISessionFactory _sessionFactory;
		//private static ISubscriptionStorage _subscriptionCache;
		//private static ISubscriptionStorage _subscriptionRepository;
		//private static SubscriptionService _subscriptionService;

		//protected static void Initialize()
		//{
		//    string connectionString = "Server=localhost;initial catalog=test;Trusted_Connection=yes";

		//    Configuration cfg = new Configuration();

		//    cfg.SetProperty("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
		//    cfg.SetProperty("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
		//    cfg.SetProperty("hibernate.connection.connection_string", connectionString);
		//    cfg.SetProperty("hibernate.dialect", "NHibernate.Dialect.MsSql2005Dialect");

		//    cfg.AddAssembly("MassTransit.ServiceBus.SubscriptionsManager");

		//    _sessionFactory = cfg.BuildSessionFactory();

		//    _subscriptionRepository = new PersistantSubscriptionStorage(_sessionFactory);

		//    _subscriptionCache = new LocalSubscriptionCache();

		//    _bus = new ServiceBus(_endpoint, _subscriptionCache);

		//    _subscriptionService = new SubscriptionService(_bus, _subscriptionCache, _subscriptionRepository);
        }
    
}