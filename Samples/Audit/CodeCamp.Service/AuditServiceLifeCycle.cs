namespace CodeCamp.Service
{
    using System;
    using Domain;
    using MassTransit.Host.LifeCycles;
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
            Container.AddComponent<Responder>();
            Container.AddComponent<RegisterUserSaga>();

            _bus = Container.Resolve<IServiceBus>("server");

            _bus.AddComponent<Responder>();

            Console.WriteLine("Service running...");
        }

        public override void Stop()
        {
            Console.WriteLine("Service exiting...");

            _bus.Dispose();
        }
    }
}