// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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

using System.Data;
using System.Data.SQLite;
using FluentNHibernate.Cfg;
using MassTransit.NHibernateIntegration.Tests.Framework;

namespace MassTransit.NHibernateIntegration.Tests.Sagas
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Magnum;
	using Magnum.Extensions;
	using MassTransit.Saga;
	using MassTransit.Saga.Pipeline;
	using MassTransit.Tests.Messages;
	using MassTransit.Tests.Saga;
	using MassTransit.Tests.Saga.Locator;
	using MassTransit.Tests.Saga.StateMachine;
	using NHibernate;
	using NHibernate.Cfg;
	using NHibernate.Tool.hbm2ddl;
	using NUnit.Framework;
	using Saga;
	using MassTransit.Tests;

	[TestFixture, Category("Integration")]
	public class When_using_the_saga_locator_with_NHibernate
	{
		[SetUp]
		public void Setup()
		{
			var cfg = Fluently.Configure(TestConfigurator.CreateConfiguration(null, c =>
			{
				c.SetProperty(NHibernate.Cfg.Environment.ShowSql, "true");
				c.SetProperty(NHibernate.Cfg.Environment.Isolation, IsolationLevel.Serializable.ToString());
			})).Mappings(m =>
			{
				m.FluentMappings.Add<ConcurrentSagaMap>();
				m.FluentMappings.Add<ConcurrentLegacySagaMap>();
			}).BuildConfiguration();

			var sessionFactory = cfg.BuildSessionFactory();

			_openConnection = new SQLiteConnection(cfg.Properties[NHibernate.Cfg.Environment.ConnectionString]);
			_openConnection.Open();
			sessionFactory.OpenSession(_openConnection);

			_sessionFactory = new SingleConnectionSessionFactory(sessionFactory, _openConnection);

			BuildSchema(cfg, _openConnection);

			_sagaId = CombGuid.Generate();
		}

		[TearDown]
		public void teardown()
		{
			if (_openConnection != null)
			{
				_openConnection.Close();
				_openConnection.Dispose();
			}

			if (_sessionFactory != null)
				_sessionFactory.Dispose();
		}
		
		private IDbConnection _openConnection;

		static void BuildSchema(Configuration config, IDbConnection connection)
		{
			new SchemaExport(config).Execute(true, true, false, connection, null);
		}

		Guid _sagaId;

		Configuration _cfg;
		ISessionFactory _sessionFactory;


		[Test]
		public void A_correlated_message_should_find_the_correct_saga()
		{
			var repository = new NHibernateSagaRepository<TestSaga>(_sessionFactory);
			var ping = new PingMessage(_sagaId);

			var initiatePolicy = new InitiatingSagaPolicy<TestSaga, InitiateSimpleSaga>(x => x.CorrelationId, x => false);

			var message = new InitiateSimpleSaga(_sagaId);
			var context = message.ToConsumeContext();

			repository.GetSaga(context, message.CorrelationId, GetHandlers, initiatePolicy)
				.Each(x => x(context));

			List<TestSaga> sagas = repository.Where(x => x.CorrelationId == _sagaId).ToList();

			Assert.AreEqual(1, sagas.Count);
			Assert.IsNotNull(sagas[0]);
			Assert.AreEqual(_sagaId, sagas[0].CorrelationId);
		}

		IEnumerable<Action<IConsumeContext<InitiateSimpleSaga>>> GetHandlers(TestSaga instance, IConsumeContext<InitiateSimpleSaga> context)
		{
			yield return x => instance.RaiseEvent(TestSaga.Initiate, x.Message);
		}

		[Test, Explicit]
		public void First_we_need_a_schema_to_test()
		{
			var schemaExport = new SchemaExport(_cfg);
			schemaExport.Drop(true, true);
			schemaExport.Create(true, true);
		}
	}
}