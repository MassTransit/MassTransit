namespace MassTransit.ServiceBus.SubscriptionsManager.Tests
{
	using System;
	using System.Data.SqlClient;
	using NHibernate;
	using NHibernate.Cfg;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;
	using Subscriptions;

	[TestFixture]
	[Explicit]
	public class IntegrationTests
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			Configuration cfg = new Configuration();

			cfg.SetProperty("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			cfg.SetProperty("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
			cfg.SetProperty("hibernate.connection.connection_string", connectionString);
			cfg.SetProperty("hibernate.dialect", "NHibernate.Dialect.MsSql2005Dialect");

			cfg.AddAssembly("MassTransit.ServiceBus.SubscriptionsManager");

			_sessionFactory = cfg.BuildSessionFactory();

			_subscriptionRepository = new PersistantSubscriptionStorage(_sessionFactory);

			_subscriptionCache = new LocalSubscriptionCache();


			_bus = new ServiceBus(_mocks.CreateMock<IEndpoint>(), _subscriptionCache);

			_subscriptionService = new SubscriptionService(_bus, _subscriptionCache, _subscriptionRepository);
		}

		[TearDown]
		public void Teardown()
		{
			_subscriptionService.Dispose();

			CleanUp();
		}

		#endregion

		private MockRepository _mocks = new MockRepository();

		private readonly string connectionString = "Server=localhost;initial catalog=test;Trusted_Connection=yes";
		private SubscriptionService _subscriptionService;
		private IServiceBus _bus;
		private ISessionFactory _sessionFactory;
		private ISubscriptionStorage _subscriptionCache;
		private ISubscriptionRepository _subscriptionRepository;

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
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();

				using (SqlCommand cmd = new SqlCommand(sql, conn))
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

		[Test]
		public void Add_Subscription_to_the_database()
		{
			_subscriptionRepository.Save(new Subscription("a", new Uri("msmq://localhost/test_client")));

			Assert.That(ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions"), Is.EqualTo(1), "Subscription count didn't match");
		}

		[Test]
		public void Remove_Subscription_From_the_Database()
		{
			_subscriptionRepository.Save(new Subscription("a", new Uri("msmq://localhost/test_client")));

			Assert.That(ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions"), Is.EqualTo(1), "Subscription count didn't match");

			_subscriptionRepository.Remove(new Subscription("a", new Uri("msmq://localhost/test_client")));

			Assert.That(ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions WHERE Message='a'"), Is.EqualTo(1), "The subscription did not exist");
			Assert.That(ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions WHERE Message='a' AND IsActive = 0"), Is.EqualTo(1), "The subscription was not marked as inactive");
		}

		[Test]
		public void Test_Add_Idempotency()
		{
			_subscriptionRepository.Save(new Subscription("a", new Uri("msmq://localhost/test_client")));
			_subscriptionRepository.Save(new Subscription("a", new Uri("msmq://localhost/test_client")));

			//casing of message shouldn't matter
			_subscriptionRepository.Save(new Subscription("A", new Uri("msmq://localhost/test_client")));

			//casing of uri shouldn't matter
			_subscriptionRepository.Save(new Subscription("a", new Uri("msmq://Localhost/test_client")));

			//spaces on message name should be ignored
			_subscriptionRepository.Save(new Subscription("a ", new Uri("msmq://localhost/test_client")));

			//spaces on uri should be ignored
			_subscriptionRepository.Save(new Subscription("a", new Uri("msmq://localhost/test_client ")));

			Assert.That(ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions"), Is.EqualTo(1), "Subscription count didn't match");
		}

		[Test]
		public void Test_Receiving_By_Message()
		{
			// bus.Send(new MessageQueueEndpoint("msmq://localhost/test_endpoint"), new SubscriptionChange(typeof (SubscriptionChange).FullName, new Uri("msmq://localhost/test_client"), SubscriptionChangeType.Add));
		}
	}
}