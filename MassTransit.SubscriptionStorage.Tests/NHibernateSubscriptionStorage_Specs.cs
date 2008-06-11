namespace MassTransit.SubscriptionStorage.Tests
{
	using System;
	using MassTransit.ServiceBus.Subscriptions;
	using NHibernate;
	using NHibernate.Cfg;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_saving_a_subscription_to_the_database
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_dbQuery = new DbQuery(_connectionString);

			_dbQuery.ExecuteCommand("DELETE FROM bus.Subscriptions");

			Configuration cfg = new Configuration();

			cfg.SetProperty("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			cfg.SetProperty("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
			cfg.SetProperty("hibernate.connection.connection_string", _connectionString);
			cfg.SetProperty("hibernate.dialect", "NHibernate.Dialect.MsSql2005Dialect");

			cfg.AddAssembly("MassTransit.SubscriptionStorage");

			_sessionFactory = cfg.BuildSessionFactory();

			_subscriptionRepository = new NHibernateSubscriptionStorage(_sessionFactory);
		}

		#endregion

		private readonly string _connectionString = "Server=localhost;initial catalog=test;Trusted_Connection=yes";
		private ISessionFactory _sessionFactory;
		private ISubscriptionRepository _subscriptionRepository;
		private DbQuery _dbQuery;

		[Test]
		public void The_subscription_should_exist_in_the_table()
		{
			Subscription subscription = new Subscription("a", new Uri("msmq://localhost/test_queue"));

			_subscriptionRepository.Save(subscription);

			Assert.That(_dbQuery.ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions"), Is.EqualTo(1), "Subscription count didn't match");
		}

		[Test]
		public void Only_one_entry_for_the_message_should_exist_in_the_table()
		{
			Subscription subscription = new Subscription("a", new Uri("msmq://localhost/test_queue"));

			_subscriptionRepository.Save(subscription);
			_subscriptionRepository.Save(subscription);
			_subscriptionRepository.Save(subscription);

			Assert.That(_dbQuery.ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions"), Is.EqualTo(1), "Subscription count didn't match");
		}

		[Test]
		public void The_case_of_type_names_and_uri_paths_should_not_matter()
		{
			_subscriptionRepository.Save(new Subscription("b", new Uri("msmq://localhost/test_client")));
			_subscriptionRepository.Save(new Subscription("b", new Uri("msmq://localhost/test_client")));

			//casing of message shouldn't matter
			_subscriptionRepository.Save(new Subscription("B", new Uri("msmq://localhost/test_client")));

			//casing of uri shouldn't matter
			_subscriptionRepository.Save(new Subscription("b", new Uri("msmq://Localhost/test_client")));

			//spaces on message name should be ignored
			_subscriptionRepository.Save(new Subscription("b ", new Uri("msmq://localhost/test_client")));

			//spaces on uri should be ignored
			_subscriptionRepository.Save(new Subscription("b", new Uri("msmq://localhost/test_client ")));

			Assert.That(_dbQuery.ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions"), Is.EqualTo(1), "Subscription count didn't match");
		}

		[Test]
		public void Removed_entries_should_be_marked_as_not_active()
		{
			_subscriptionRepository.Save(new Subscription("a", new Uri("msmq://localhost/test_client")));

			Assert.That(_dbQuery.ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions"), Is.EqualTo(1), "Subscription count didn't match");

			_subscriptionRepository.Remove(new Subscription("a", new Uri("msmq://localhost/test_client")));

			Assert.That(_dbQuery.ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions WHERE Message='a'"), Is.EqualTo(1), "The subscription did not exist");
			Assert.That(_dbQuery.ExecuteScalar("SELECT COUNT(*) FROM bus.Subscriptions WHERE Message='a' AND IsActive = 0"), Is.EqualTo(1), "The subscription was not marked as inactive");

		}
	}
}