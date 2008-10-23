namespace CodeCamp.Service
{
    using System;
    using Domain;
    using MassTransit.Host.LifeCycles;
    using MassTransit.ServiceBus;
    using Microsoft.Practices.ServiceLocation;

    public class AuditServiceLifeCycle :
        HostedLifeCycle
    {
        private IServiceBus _bus;

        public AuditServiceLifeCycle(IServiceLocator serviceLocator):base(serviceLocator)
        {
        }

        public override void Start()
        {
            _bus = ServiceLocator.GetInstance<IServiceBus>("server");

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