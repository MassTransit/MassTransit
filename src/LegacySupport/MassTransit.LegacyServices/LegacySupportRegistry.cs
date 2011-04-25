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
namespace LegacyRuntime
{
	using FluentNHibernate.Cfg;
	using FluentNHibernate.Cfg.Db;
	using MassTransit;
	using MassTransit.Infrastructure.Saga;
	using MassTransit.Saga;
	using MassTransit.Services.HealthMonitoring.Configuration;
	using MassTransit.StructureMapIntegration;
	using Model;
	using NHibernate;
	using StructureMap;

	public class LegacySupportRegistry :
		MassTransitRegistryBase
	{
		readonly IContainer _container;

		public LegacySupportRegistry(IContainer container)
		{
			_container = container;


			var configuration = container.GetInstance<IConfiguration>();


			For<ISessionFactory>().Singleton().Use(context => CreateSessionFactory());

			For(typeof (ISagaRepository<>)).Use(typeof (NHibernateSagaRepositoryForContainers<>));

			RegisterServiceBus(configuration.LegacyServiceDataUri, x =>
				{
					x.UseControlBus();
					x.SetConcurrentConsumerLimit(1);

					x.UseSubscriptionService(configuration.SubscriptionServiceUri);

					x.ConfigureService<HealthClientConfigurator>(health => health.SetHeartbeatInterval(10));
				});
		}

		static ISessionFactory CreateSessionFactory()
		{
			return Fluently.Configure()
				.Mappings(m => { m.FluentMappings.Add<LegacySubscriptionClientSagaMap>(); })
				.Database(MsSqlConfiguration.MsSql2005.ConnectionString(c => c.FromConnectionStringWithKey("MassTransit")))
				.BuildSessionFactory();
		}
	}
}