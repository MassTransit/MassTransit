namespace MassTransit.ServiceBus.SubscriptionsManager.Tests
{
    using System.Data.SqlClient;
    using NHibernate;
    using NHibernate.Cfg;
    using NUnit.Framework;

    [TestFixture]
    [Explicit]
    public class IntegrationTests
    {
        SubscriptionRepository repo;
        private readonly string connectionString = "Server=localhost;initial catalog=test;Trusted_Connection=yes";

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
        }

        [TearDown]
        public void Teardown()
        {
            repo = null;
            CleanUp();
        }

        [Test]
        public void Add_Subscription_to_the_database()
        {
            repo.Add(new Subscription("a","m"));
            AssertSubscriptionInDatabase();
        }

        [Test]
        public void Remove_Subscription_From_the_Database()
        {
            repo.Add(new Subscription("a", "m"));
            AssertSubscriptionInDatabase();

            repo.Deactivate(new Subscription("a", "m"));
            AssertSubscriptionInactive();
        }

        public void AssertSubscriptionInDatabase()
        {
            Assert.AreEqual(1, ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions"));
        }

        public void AssertSubscriptionActive()
        {
            Assert.AreEqual(true, ExecuteScalar("SELECT IsActive FROM bus.Subscriptions"));
        }

        public void AssertSubscriptionInactive()
        {
            Assert.AreEqual(false, ExecuteScalar("SELECT IsActive FROM bus.Subscriptions"));
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