namespace CodeCamp.Service
{
    using System;
    using Domain;
    using MassTransit.Host.LifeCycles;
    using MassTransit.ServiceBus;
    using Microsoft.Practices.ServiceLocation;

    public class AuditServiceLifeCycle :
        HostedLifecycle
    {
        private IServiceBus _bus;

        public AuditServiceLifeCycle(IServiceLocator serviceLocator):base(serviceLocator)
        {
        }

        public override void Start()
        {
            _bus = ServiceLocator.GetInstance<IServiceBus>("server");

            _bus.Subscribe<Responder>();
            _bus.Subscribe<RegisterUserSaga>();

            Console.WriteLine("Service running...");
        }

        public override void Stop()
        {
            Console.WriteLine("Service exiting...");

            _bus.Dispose();
        }
    }
}