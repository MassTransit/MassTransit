namespace CodeCamp.Service
{
    using System;
    using MassTransit.Host.LifeCycles;
    using MassTransit.ServiceBus;

    public class AuditServiceLifeCycle :
        HostedLifeCycle
    {
        public AuditServiceLifeCycle(string xmlFile) : base(xmlFile)
        {
        }

        public override void Start()
        {
            this.Container.AddComponent<Responder>();
            this.Container.Resolve<IServiceBus>().AddComponent<Responder>();

            Console.WriteLine("Service running...");
        }

        public override void Stop()
        {
            Console.WriteLine("Service exiting...");
            this.Container.Resolve<IServiceBus>().RemoveComponent<Responder>();
        }
    }
}