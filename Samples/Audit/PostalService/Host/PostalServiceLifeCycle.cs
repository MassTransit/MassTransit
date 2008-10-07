namespace PostalService.Host
{
	using System;
	using MassTransit.Host.LifeCycles;
    using MassTransit.ServiceBus;

    public class PostalServiceLifeCycle :
        HostedLifeCycle
    {
        private IServiceBus _bus;

        public PostalServiceLifeCycle(string xmlFile)
            : base(xmlFile)
        {
        }

        public override void Start()
        {
            Container.AddComponent<SendEmailConsumer>("sec");

            _bus = Container.Resolve<IServiceBus>("server");

            _bus.AddComponent<SendEmailConsumer>();

            Console.WriteLine("Service running...");
        }

        public override void Stop()
        {
            Console.WriteLine("Service exiting...");

            _bus.Dispose();
        }
    }
}