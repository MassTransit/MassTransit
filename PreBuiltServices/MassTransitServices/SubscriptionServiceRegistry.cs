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
namespace MassTransit.RuntimeServices
{
	using System.Data;
	using FluentNHibernate.Cfg;
	using FluentNHibernate.Cfg.Db;
	using Infrastructure.Saga;
	using Infrastructure.Subscriptions;
	using Model;
	using NHibernate;
	using NHibernate.Cfg;
	using NHibernate.Tool.hbm2ddl;
	using Saga;
	using Services.Subscriptions.Server;
	using StructureMap;
	using StructureMap.Attributes;
	using StructureMapIntegration;
	using Transports;
	using Transports.Msmq;

	public class SubscriptionServiceRegistry :
		MassTransitRegistryBase
	{
		public SubscriptionServiceRegistry(IContainer container)
			: base(typeof (MsmqEndpoint), typeof (LoopbackEndpoint))
		{
			var configuration = container.GetInstance<IConfiguration>();

			ForRequestedType<ISessionFactory>()
				.CacheBy(InstanceScope.Singleton)
				.TheDefault.Is.ConstructedBy(context => CreateSessionFactory());


			ForRequestedType(typeof (ISagaRepository<>))
				.AddConcreteType(typeof (NHibernateSagaRepositoryForContainers<>));
			ForRequestedType<ISubscriptionRepository>()
				.AddConcreteType<PersistantSubscriptionRepository>();

			RegisterServiceBus(configuration.SubscriptionServiceUri, x => { });
		}

		private static ISessionFactory CreateSessionFactory()
		{
			return Fluently.Configure()
				.Database(
				MsSqlConfiguration.MsSql2005
					.AdoNetBatchSize(100)
					.ConnectionString(s => s.FromConnectionStringWithKey("MassTransit"))
					.DefaultSchema("dbo")
					.ShowSql()
					.Raw(Environment.Isolation, IsolationLevel.Serializable.ToString()))
				.Mappings(m =>
					{
						m.FluentMappings.Add<SubscriptionSagaMap>();
						m.FluentMappings.Add<SubscriptionClientSagaMap>();
					})
				.ExposeConfiguration(BuildSchema)
				.BuildSessionFactory();
		}

		private static void BuildSchema(NHibernate.Cfg.Configuration config)
		{
			new SchemaUpdate(config).Execute(false, true);
		}
	}
}