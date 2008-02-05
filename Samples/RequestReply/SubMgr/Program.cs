namespace SubMgr
{
    using System;
    using log4net.Config;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.ServiceBus.SubscriptionsManager;
    using NHibernate;
    using NHibernate.Cfg;

    internal class Program
    {
        private static readonly IMessageQueueEndpoint _endpoint = new MessageQueueEndpoint("msmq://localhost/test_subscriptions");
        private static IServiceBus _bus;
        private static ISessionFactory _sessionFactory;
        private static ISubscriptionStorage _subscriptionCache;
        private static ISubscriptionStorage _subscriptionRepository;
        private static SubscriptionService _subscriptionService;

        protected static void Initialize()
        {
            string connectionString = "Server=localhost;initial catalog=test;Trusted_Connection=yes";

            Configuration cfg = new Configuration();

            cfg.SetProperty("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            cfg.SetProperty("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
            cfg.SetProperty("hibernate.connection.connection_string", connectionString);
            cfg.SetProperty("hibernate.dialect", "NHibernate.Dialect.MsSql2005Dialect");

            cfg.AddAssembly("MassTransit.ServiceBus.SubscriptionsManager");

            _sessionFactory = cfg.BuildSessionFactory();

            _subscriptionRepository = new SubscriptionRepository(_sessionFactory);

            _subscriptionCache = new LocalSubscriptionCache();

            _bus = new ServiceBus(_endpoint, _subscriptionCache);

            _subscriptionService = new SubscriptionService(_bus, _subscriptionCache, _subscriptionRepository);
        }


        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            Initialize();

            _subscriptionService.Start(args);


            Console.WriteLine("Started...");
            Console.ReadLine();

            _subscriptionService.Stop();
        }
    }
}