namespace MassTransit.Infrastructure.Tests.Sagas
{
	using System;
	using Magnum;
	using Magnum.Data;
	using Magnum.Infrastructure.Data;
	using MassTransit.Tests.Saga.Messages;
	using MassTransit.Tests.Saga.StateMachine;
	using NHibernate;
	using NHibernate.Cfg;
	using NHibernate.Tool.hbm2ddl;
	using NUnit.Framework;
	using Saga;

	[TestFixture, Category("Integration")]
	public class SagaRepository_Specs
	{
		[SetUp]
		public void Setup()
		{
			_cfg = new Configuration();

			_cfg.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			_cfg.SetProperty("connection.driver_class", "NHibernate.Driver.SqlClientDriver");
			_cfg.SetProperty("connection.connection_string", _connectionString);
			_cfg.SetProperty("dialect", "NHibernate.Dialect.MsSql2005Dialect");
			_cfg.SetProperty("default_schema", "bus");

			_cfg.AddAssembly(typeof (NHibernateSagaRepository<>).Assembly);
			_cfg.AddAssembly(typeof (RegisterUserStateMachine).Assembly);
			_cfg.AddAssembly(typeof (SagaRepository_Specs).Assembly);

			ISessionFactory _sessionFactory = _cfg.BuildSessionFactory();

			LocalContext.Current.Store(_sessionFactory);

			NHibernateUnitOfWork.SetSessionProvider(() => LocalContext.Current.Retrieve<ISessionFactory>().OpenSession());

			UnitOfWork.SetUnitOfWorkProvider(NHibernateUnitOfWork.Create);
		}

		[Test, Explicit]
		public void First_we_need_a_schema_to_test()
		{
			new SchemaExport(_cfg).Create(true, true);
		}

/*
		[Test]
		public void I_should_be_able_to_store_and_load_a_saga_and_not_lose_state()
		{
			Guid transactionId = CombGuid.Generate();

			using (var repository = new NHibernateSagaRepository<RegisterUserStateMachine>())
			{
				using (var enumerator = repository.InitiateNewSaga(transactionId))
				{
					while (enumerator.MoveNext())
					{
						enumerator.Current.Consume(new RegisterUser(transactionId, "Joe", "pass", "Joe Blow", "joe@bloe.com"));
					}
				}
			}

			using (var repository = new NHibernateSagaRepository<RegisterUserStateMachine>())
			{
				using (var enumerator = repository.OrchestrateExistingSaga(transactionId))
				{
					while (enumerator.MoveNext())
					{
						Assert.AreEqual(RegisterUserStateMachine.WaitingForEmailValidation, enumerator.Current.CurrentState);

						enumerator.Current.Consume(new UserValidated(transactionId));
					}
				}
			}
		}
*/

		private const string _connectionString = "Server=localhost;initial catalog=test;Trusted_Connection=yes";
		private Configuration _cfg;
	}
}