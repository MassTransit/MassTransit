namespace CodeCamp.Service
{
    using System;
    using Domain;
    using Infrastructure;
    using Magnum.Common.Repository;
    using Magnum.Infrastructure.Repository;
    using MassTransit.Host.LifeCycles;
    using MassTransit.Saga;
    using MassTransit.ServiceBus;

    public class AuditServiceLifeCycle :
        HostedLifeCycle
    {
        private IServiceBus _bus;

        public AuditServiceLifeCycle(string xmlFile) : base(xmlFile)
        {
        }

        public override void Start()
        {
            Container.Kernel.AddComponentInstance<IRepositoryFactory>(NHibernateRepositoryFactory.Build());

            Container.AddComponent("repository", typeof (IRepository<,>), typeof (NHibernateRepository<,>));

            Container.AddComponent<ISagaRepository<RegisterUserSaga>, SagaRepository<RegisterUserSaga>>();

            Container.AddComponent<Responder>();
            Container.AddComponent<RegisterUserSaga>();

            _bus = Container.Resolve<IServiceBus>("server");

            _bus.AddComponent<Responder>();
            _bus.AddComponent<RegisterUserSaga>();

            Console.WriteLine("Service running...");
        }

        public override void Stop()
        {
            Console.WriteLine("Service exiting...");

            _bus.Dispose();
        }
    }
}