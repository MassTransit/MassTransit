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
namespace MassTransitServices
{
	using System.Data;
	using FluentNHibernate.Cfg;
	using FluentNHibernate.Cfg.Db;
	using MassTransit;
	using MassTransit.Infrastructure.Timeout;
	using MassTransit.Services.Timeout;
	using MassTransit.StructureMapIntegration;
	using MassTransit.Transports;
	using MassTransit.Transports.Msmq;
	using Model;
	using NHibernate;
	using NHibernate.Cfg;
	using NHibernate.Tool.hbm2ddl;
	using StructureMap;
	using StructureMap.Attributes;

	public class TimeoutServiceRegistry :
		MassTransitRegistryBase
	{
		private readonly IContainer _container;

		public TimeoutServiceRegistry(IContainer container)
			: base(typeof (MsmqEndpoint), typeof (LoopbackEndpoint))
		{
			_container = container;

			var configuration = container.GetInstance<IConfiguration>();

			ForRequestedType<ISessionFactory>()
				.CacheBy(InstanceScope.Singleton)
				.TheDefault.Is.ConstructedBy(context => CreateSessionFactory());

			ForRequestedType<ITimeoutRepository>()
				.AddConcreteType<NHibernateTimeoutRepository>();

			RegisterControlBus(configuration.TimeoutServiceControlUri, x => { x.SetConcurrentConsumerLimit(1); });

			RegisterServiceBus(configuration.TimeoutServiceDataUri, x =>
				{
					x.UseControlBus(_container.GetInstance<IControlBus>());

					ConfigureSubscriptionClient(configuration.SubscriptionServiceUri, x);
				});
		}

		private ISessionFactory CreateSessionFactory()
		{
			return Fluently.Configure()
				.Database(
				MsSqlConfiguration.MsSql2005
					.AdoNetBatchSize(100)
					.ConnectionString(s => s.FromConnectionStringWithKey("MassTransit"))
					.DefaultSchema("dbo")
					.ShowSql()
					.Raw(Environment.Isolation, IsolationLevel.Serializable.ToString()))
				.Mappings(m => { m.FluentMappings.Add<ScheduledTimeoutMap>(); })
				.ExposeConfiguration(BuildSchema)
				.BuildSessionFactory();
		}

		private void BuildSchema(NHibernate.Cfg.Configuration config)
		{
			new SchemaExport(config)
				.Create(false, true);
		}
	}
}