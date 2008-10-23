namespace CodeCamp.Service
{
    using Domain;
    using Infrastructure;
    using Magnum.Common.Repository;
    using Magnum.Infrastructure.Repository;
    using MassTransit.Host;
    using MassTransit.Saga;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;

    internal class Program
    {
        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var container = new DefaultMassTransitContainer("audit-castle.config");
            container.Kernel.AddComponentInstance<IRepositoryFactory>(NHibernateRepositoryFactory.Build());

            container.AddComponent("repository", typeof(IRepository<,>), typeof(NHibernateRepository<,>));

            container.AddComponent<ISagaRepository<RegisterUserSaga>, SagaRepository<RegisterUserSaga>>();

            container.AddComponent<Responder>();
            container.AddComponent<RegisterUserSaga>();

            var wob = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(() => wob);
            var cfg = new AuditServiceConfiguration(ServiceLocator.Current);
            Runner.Run(cfg, args);
        }
    }
}