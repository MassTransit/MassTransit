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
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using FluentNHibernate.Cfg;
	using FluentNHibernate.Cfg.Db;
	using log4net;
	using Magnum;
	using MassTransit.Saga;
	using MassTransit.Tests.TextFixtures;
	using NHibernate;
	using NHibernate.Cfg;
	using NHibernate.Tool.hbm2ddl;
	using NUnit.Framework;
	using Saga;
	using Environment=NHibernate.Cfg.Environment;

	[TestFixture]
	public class Sending_multiple_messages_to_the_same_saga_at_the_same_time :
		LoopbackTestFixture
	{
		private static readonly ILog _log = LogManager.GetLogger("MassTransit.UnitTest");

		private ISagaRepository<ConcurrentSaga> _sagaRepository;

		protected override void EstablishContext()
		{
			base.EstablishContext();

			ISessionFactory sessionFactory = Fluently.Configure()
				.Database(
				MsSqlConfiguration.MsSql2005
					.AdoNetBatchSize(100)
					.ConnectionString(s => s.Is("Server=(local);initial catalog=test;Trusted_Connection=yes"))
					.DefaultSchema("dbo")
					.ShowSql()
					.ProxyFactoryFactory("NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle")
					.Raw(Environment.Isolation, IsolationLevel.ReadCommitted.ToString()))
				.Mappings(m => { m.FluentMappings.Add<ConcurrentSagaMap>(); })
				.ExposeConfiguration(BuildSchema)
				.BuildSessionFactory();


			_sagaRepository = new NHibernateSagaRepositoryForContainers<ConcurrentSaga>(sessionFactory);
			//_sagaRepository = SetupSagaRepository<ConcurrentSaga>();
			SetupInitiateSagaStateMachineSink<ConcurrentSaga, StartConcurrentSaga>(LocalBus, _sagaRepository, ObjectBuilder);
			SetupOrchestrateSagaStateMachineSink<ConcurrentSaga, ContinueConcurrentSaga>(LocalBus, _sagaRepository, ObjectBuilder);
		}

		private static void BuildSchema(Configuration config)
		{
			new SchemaExport(config).Create(false, true);
		}

		[Test]
		public void Should_process_the_messages_in_order_and_not_at_the_same_time()
		{
			UnsubscribeAction unsubscribeAction = LocalBus.Subscribe<ConcurrentSaga>();

			Guid transactionId = CombGuid.Generate();

			Trace.WriteLine("Creating transaction for " + transactionId);

			int startValue = 1;

			var startConcurrentSaga = new StartConcurrentSaga {CorrelationId = transactionId, Name = "Chris", Value = startValue};

			LocalBus.Publish(startConcurrentSaga);
			Trace.WriteLine("Just published the start message");

			Thread.Sleep(500);

			int nextValue = 2;
			var continueConcurrentSaga = new ContinueConcurrentSaga {CorrelationId = transactionId, Value = nextValue};

			LocalBus.Publish(continueConcurrentSaga);
			Trace.WriteLine("Just published the continue message");
			Thread.Sleep(8000);

			unsubscribeAction();
			foreach (ConcurrentSaga saga in _sagaRepository.Where(x => true))
			{
				Trace.WriteLine("Found saga: " + saga.CorrelationId);
			}

			int currentValue = _sagaRepository.Where(x => x.CorrelationId == transactionId).First().Value;

			Assert.AreEqual(nextValue, currentValue);
		}
	}

	[TestFixture]
	public class Sending_multiple_messages_to_the_same_saga_legacy_at_the_same_time :
		LoopbackTestFixture
	{
		private static readonly ILog _log = LogManager.GetLogger("MassTransit.UnitTest");

		private ISagaRepository<ConcurrentLegacySaga> _sagaRepository;

		protected override void EstablishContext()
		{
			base.EstablishContext();

			ISessionFactory sessionFactory = Fluently.Configure()
				.Database(
				MsSqlConfiguration.MsSql2005
					.AdoNetBatchSize(100)
					.ConnectionString(s => s.Is("Server=(local);initial catalog=test;Trusted_Connection=yes"))
					.DefaultSchema("dbo")
					.ShowSql()
					.Raw(Environment.Isolation, IsolationLevel.ReadCommitted.ToString())
					.ProxyFactoryFactory("NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle"))
				.Mappings(m => { m.FluentMappings.Add<ConcurrentLegacySagaMap>(); })
				.ExposeConfiguration(BuildSchema)
				.BuildSessionFactory();


			_sagaRepository = new NHibernateSagaRepositoryForContainers<ConcurrentLegacySaga>(sessionFactory);
			//_sagaRepository = SetupSagaRepository<ConcurrentSaga>();
			SetupInitiateSagaSink<ConcurrentLegacySaga, StartConcurrentSaga>(LocalBus, _sagaRepository, ObjectBuilder);
			SetupOrchestrateSagaSink<ConcurrentLegacySaga, ContinueConcurrentSaga>(LocalBus, _sagaRepository, ObjectBuilder);
		}

		private static void BuildSchema(Configuration config)
		{
			new SchemaExport(config).Create(false, true);
		}

		[Test]
		public void Should_process_the_messages_in_order_and_not_at_the_same_time()
		{
			UnsubscribeAction unsubscribeAction = LocalBus.Subscribe<ConcurrentLegacySaga>();

			Guid transactionId = CombGuid.Generate();

			Trace.WriteLine("Creating transaction for " + transactionId);

			const int startValue = 1;

			var startConcurrentSaga = new StartConcurrentSaga {CorrelationId = transactionId, Name = "Chris", Value = startValue};

			LocalBus.Publish(startConcurrentSaga);
			Trace.WriteLine("Just published the start message");

			Thread.Sleep(500);

			const int nextValue = 2;
			var continueConcurrentSaga = new ContinueConcurrentSaga {CorrelationId = transactionId, Value = nextValue};

			LocalBus.Publish(continueConcurrentSaga);
			Trace.WriteLine("Just published the continue message");
			Thread.Sleep(8000);

			unsubscribeAction();
			foreach (ConcurrentLegacySaga saga in _sagaRepository.Where(x => true))
			{
				Trace.WriteLine("Found saga: " + saga.CorrelationId);
			}

			int currentValue = _sagaRepository.Where(x => x.CorrelationId == transactionId).First().Value;

			Assert.AreEqual(nextValue, currentValue);
		}
	}
}