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

using FluentNHibernate.Cfg.Db;
using NHibernate.Dialect;

namespace MassTransit.RuntimeServices
{
	using System.IO;
	using FluentNHibernate.Cfg;
	using Model;
	using NHibernate;
	using NHibernate.Tool.hbm2ddl;
	using NHibernateIntegration.Saga;
	using Saga;
	using Services.Subscriptions.Server;
	using StructureMap;
	using StructureMap.Configuration.DSL;

	public class SubscriptionServiceRegistry :
		Registry
	{
		public SubscriptionServiceRegistry(IContainer container)
		{
			var configuration = container.GetInstance<IConfiguration>();

			For<ISessionFactory>()
				.Singleton()
				.Use(context => CreateSessionFactory());

			For(typeof (ISagaRepository<>))
				.Add(typeof (NHibernateSagaRepository<>));

			For<IServiceBus>()
				.Singleton()
				.Use(context =>
				{
					return ServiceBusFactory.New(sbc =>
					{
						sbc.ReceiveFrom(configuration.SubscriptionServiceUri);

						sbc.UseMsmq();

						sbc.SetConcurrentConsumerLimit(1);
					});
				});
		}

		static ISessionFactory CreateSessionFactory()
		{
			return Fluently.Configure()
				.Mappings(m =>
					{
						m.FluentMappings.Add<SubscriptionSagaMap>();
						m.FluentMappings.Add<SubscriptionClientSagaMap>();
					}).Database(()=>
					{
					    return MsSqlConfiguration.MsSql2008.ConnectionString(cfg =>
					    {
                            cfg.FromConnectionStringWithKey("MassTransit");
					    });
					})
				//.ExposeConfiguration(BuildSchema)
				.BuildSessionFactory();
		}

		static void BuildSchema(NHibernate.Cfg.Configuration config)
		{
			new SchemaUpdate(config).Execute(false, true);

			string schemaFile = Path.Combine(Path.GetDirectoryName(typeof (SubscriptionService).Assembly.Location),
				typeof (SubscriptionService).Name + ".sql");

			new SchemaExport(config).SetOutputFile(schemaFile).Execute(false, false, false);
		}
	}
}