namespace LegacyRuntime
{
    using System.IO;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using MassTransit;
    using MassTransit.Infrastructure.Saga;
    using MassTransit.Saga;
    using MassTransit.Services.HealthMonitoring.Configuration;
    using MassTransit.StructureMapIntegration;
    using Model;
    using NHibernate;
    using NHibernate.Dialect;
    using NHibernate.Tool.hbm2ddl;
    using StructureMap;

    public class LegacySupportRegistry :
        MassTransitRegistryBase
    {

        private readonly IContainer _container;

        public LegacySupportRegistry(IContainer container)
        {
            _container = container;


            var configuration = container.GetInstance<IConfiguration>();


            For<ISessionFactory>().Singleton().Use(context => CreateSessionFactory());

            For(typeof(ISagaRepository<>)).Use(typeof(NHibernateSagaRepositoryForContainers<>));

            RegisterControlBus(configuration.LegacyServiceControlUri, x => { x.SetConcurrentConsumerLimit(1); });

            RegisterServiceBus(configuration.LegacyServiceDataUri, x =>
            {
                x.UseControlBus(_container.GetInstance<IControlBus>());
                x.SetConcurrentConsumerLimit(1);

                ConfigureSubscriptionClient(configuration.SubscriptionServiceUri, x);

                x.ConfigureService<HealthClientConfigurator>(health => health.SetHeartbeatInterval(10));
            });
        }

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Mappings(m => { m.FluentMappings.Add<LegacySubscriptionClientSagaMap>(); })
                .Database(MsSqlConfiguration.MsSql2005.ConnectionString(c => c.FromConnectionStringWithKey("MassTransit")))
                .BuildSessionFactory();
        }
    }
}