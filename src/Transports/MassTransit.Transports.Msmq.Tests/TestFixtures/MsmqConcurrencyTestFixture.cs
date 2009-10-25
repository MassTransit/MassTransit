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
namespace MassTransit.Transports.Msmq.Tests.TestFixtures
{
	using System.Data;
	using FluentNHibernate.Cfg;
	using FluentNHibernate.Cfg.Db;
	using Infrastructure.Tests.Sagas;
	using NHibernate;
	using NHibernate.Cfg;
	using NHibernate.Tool.hbm2ddl;
	using NUnit.Framework;

	[TestFixture, Category("Integration")]
	public class MsmqConcurrentSagaTestFixtureBase :
		MsmqTransactionalEndpointTestFixture
	{
		protected ISessionFactory SessionFactory { get; private set; }

		protected override void EstablishContext()
		{
			base.EstablishContext();

			SessionFactory = Fluently.Configure()
				.Database(
				MsSqlConfiguration.MsSql2005
					.AdoNetBatchSize(100)
					.ConnectionString(s => s.Is("Server=(local);initial catalog=test;Trusted_Connection=yes"))
					.DefaultSchema("dbo")
					.ShowSql()
					.ProxyFactoryFactory("NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle")
					.Raw(Environment.Isolation, IsolationLevel.RepeatableRead.ToString()))
				.Mappings(m =>
					{
						m.FluentMappings.Add<ConcurrentSagaMap>();
						m.FluentMappings.Add<ConcurrentLegacySagaMap>();
					})
				.ExposeConfiguration(BuildSchema)
				.BuildSessionFactory();
		}

		private static void BuildSchema(Configuration config)
		{
			new SchemaExport(config).Create(false, true);
		}
	}
}