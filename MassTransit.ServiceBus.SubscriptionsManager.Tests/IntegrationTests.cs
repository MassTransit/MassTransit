namespace MassTransit.ServiceBus.SubscriptionsManager.Tests
{
    using System;
    using System.Data.SqlClient;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using NHibernate;
    using NHibernate.Cfg;
    using NUnit.Framework;

    [TestFixture]
    [Explicit]
    public class IntegrationTests
    {
        SubscriptionRepository repo;
        private readonly string connectionString = "Server=localhost;initial catalog=test;Trusted_Connection=yes";
        private SubscriptionServiceBus bus;

        [SetUp]
        public void Setup()
        {
            NHibernate.Cfg.Configuration config = new Configuration();
            config.SetProperty("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            config.SetProperty("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
            config.SetProperty("hibernate.connection.connection_string", connectionString);
            config.SetProperty("hibernate.dialect", "NHibernate.Dialect.MsSql2005Dialect");

            config.AddAssembly("MassTransit.ServiceBus.SubscriptionsManager");

            ISessionFactory sessFactory = config.BuildSessionFactory();

            repo = new SubscriptionRepository(sessFactory);

            bus = new SubscriptionServiceBus(new MessageQueueEndpoint("msmq://localhost/test_endpoint"), repo);
        }

        [TearDown]
        public void Teardown()
        {
            repo = null;
            bus.Dispose();
            CleanUp();
        }

        [Test]
        public void Add_Subscription_to_the_database()
        {
            repo.Add("a", new Uri("msmq://localhost/test_client"));
            AssertSubscriptionInDatabase();
        }

        [Test]
        public void Remove_Subscription_From_the_Database()
        {
            repo.Add("a", new Uri("msmq://localhost/test_client"));
            AssertSubscriptionInDatabase();

            repo.Remove("a", new Uri("msmq://localhost/test_client"));
            AssertSubscriptionInactive();
        }

        [Test]
        public void Test_Receiving_By_Message()
        {
            bus.Send(new MessageQueueEndpoint("msmq://localhost/test_endpoint"), new SubscriptionChange(typeof(SubscriptionChange).FullName, new Uri("msmq://localhost/test_client"), SubscriptionChangeType.Add));
        }

        public void AssertSubscriptionInDatabase()
        {
            Assert.AreEqual(4, ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions"), "Subscription count didn't match");
        }

        public void AssertSubscriptionActive()
        {
            Assert.AreEqual(true, ExecuteScalar("SELECT IsActive FROM bus.Subscriptions WHERE Message='a'"), "Subscription was not active");
        }

        public void AssertSubscriptionInactive()
        {
            Assert.AreEqual(false, ExecuteScalar("SELECT IsActive FROM bus.Subscriptions WHERE Message='a'"), "Subscription was active");
        }

        public void CleanUp()
        {
            ExecuteCmd("DELETE FROM bus.Subscriptions");
        }

        private int ExecuteCmd(string sql)
        {
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using(SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        private object ExecuteScalar(string sql)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}