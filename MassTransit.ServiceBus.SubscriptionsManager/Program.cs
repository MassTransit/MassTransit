namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using System.ServiceProcess;
    using NHibernate;
    using NHibernate.Cfg;

    public class Program : ServiceBase
    {
        private SubscriptionServiceBus bus;

        public void StartItUp()
        {
            string connectionString = "Server=localhost;initial catalog=test;Trusted_Connection=yes";

            Configuration cfg = new Configuration();

            cfg.SetProperty("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            cfg.SetProperty("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
            cfg.SetProperty("hibernate.connection.connection_string", connectionString);
            cfg.SetProperty("hibernate.dialect", "NHibernate.Dialect.MsSql2005Dialect");

            cfg.AddAssembly("MassTransit.ServiceBus.SubscriptionsManager");
            
            ISessionFactory sessionFactory = cfg.BuildSessionFactory();
            bus = new SubscriptionServiceBus(new MessageQueueEndpoint("msmq://localhost/test_subscriptions"), new SubscriptionRepository(sessionFactory));
        }
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            StartItUp();
            
        }

        protected override void OnStop()
        {
            base.OnStop();
            bus.Dispose();
        }
    }
}