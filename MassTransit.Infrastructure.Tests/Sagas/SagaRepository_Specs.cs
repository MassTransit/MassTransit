// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Infrastructure.Tests.Sagas
{
	using System;
	using System.Data;
	using Magnum.Common;
	using Magnum.Common.Data;
	using Magnum.Infrastructure.Data;
	using MassTransit.Tests.Saga.Messages;
	using MassTransit.Tests.Saga.StateMachine;
	using NHibernate;
	using NHibernate.Cfg;
	using NHibernate.Tool.hbm2ddl;
	using NUnit.Framework;
	using Saga;
	using Util;

	[TestFixture]
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

		private const string _connectionString = "Server=localhost;initial catalog=test;Trusted_Connection=yes";
		private Configuration _cfg;

		[Test, Explicit]
		public void First_we_need_a_schema_to_test()
		{
			new SchemaExport(_cfg).Create(true, true);
		}

		[Test]
		public void I_should_be_able_to_store_and_load_a_saga_and_not_lose_state()
		{
			Guid transactionId = CombGuid.NewCombGuid();

			using (var unitOfWork = UnitOfWork.Start())
			using (var transaction = unitOfWork.BeginTransaction(IsolationLevel.Serializable))
			{
				using (var repository = new NHibernateSagaRepository<RegisterUserStateMachine>())
				{
					var stateMachine = repository.Create(transactionId);

					stateMachine.Consume(new RegisterUser(transactionId, "Joe", "pass", "Joe Blow", "joe@bloe.com"));

					repository.Save(stateMachine);
				}

				transaction.Commit();
			}
			UnitOfWork.Finish();


			using (var unitOfWork = UnitOfWork.Start())
			using (var transaction = unitOfWork.BeginTransaction(IsolationLevel.Serializable))
			{
				using (var repository = new NHibernateSagaRepository<RegisterUserStateMachine>())
				{
					var instance = repository.Get(transactionId);

					Assert.AreEqual(RegisterUserStateMachine.WaitingForEmailValidation, instance.CurrentState);

					instance.Consume(new UserValidated(transactionId));

					repository.Save(instance);
				}

				transaction.Commit();
			}
			UnitOfWork.Finish();
		}
	}
}