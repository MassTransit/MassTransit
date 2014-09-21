// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.IO;
    using Log4NetIntegration;
    using Model;
    using NHibernate;
    using NHibernate.Tool.hbm2ddl;
    using NHibernateIntegration;
    using NHibernateIntegration.Saga;
    using Saga;
    using Services.HealthMonitoring;
    using StructureMap;
    using StructureMap.Configuration.DSL;

    public class HealthServiceRegistry :
        Registry
    {
        public HealthServiceRegistry(IContainer container)
        {
            var configuration = container.GetInstance<IConfiguration>();

            For<ISessionFactory>()
                .Singleton()
                .Use(context => CreateSessionFactory());

            For(typeof(ISagaRepository<>))
                .Add(typeof(NHibernateSagaRepository<>));

            For<IServiceBus>()
                .Singleton()
                .Use(context =>CreateServiceBus(configuration));
        }

        static IServiceBus CreateServiceBus(IConfiguration configuration)
        {
            return ServiceBusFactory.New(sbc =>
            {
                sbc.ReceiveFrom(configuration.HealthServiceDataUri);
                sbc.UseControlBus();
                sbc.UseLog4Net();

                sbc.UseMsmq(x => x.UseSubscriptionService(configuration.SubscriptionServiceUri));

                sbc.SetConcurrentConsumerLimit(1);
            });
        }

        static ISessionFactory CreateSessionFactory()
        {
            var provider = new NHibernateSessionFactoryProvider(new[]
                {
                    typeof(HealthSagaMap),
                });

            return provider.GetSessionFactory();
        }

        static void BuildSchema(NHibernate.Cfg.Configuration config)
        {
            new SchemaUpdate(config).Execute(false, true);

            string schemaFile = Path.Combine(Path.GetDirectoryName(typeof(HealthService).Assembly.Location),
                typeof(HealthService).Name + ".sql");

            new SchemaExport(config).SetOutputFile(schemaFile).Execute(false, false, false);
        }
    }
}