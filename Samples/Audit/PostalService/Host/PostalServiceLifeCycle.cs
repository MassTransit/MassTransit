namespace PostalService.Host
{
	using System;
	using MassTransit.Host.LifeCycles;
    using MassTransit.ServiceBus;
	using Microsoft.Practices.ServiceLocation;

    public class PostalServiceLifeCycle :
        HostedLifecycle
    {
        private IServiceBus _bus;

        public PostalServiceLifeCycle(IServiceLocator serviceLocator):base(serviceLocator)
        {
        }

        public override void Start()
        {
            

            _bus = base.ServiceLocator.GetInstance<IServiceBus>("server");

            _bus.Subscribe<SendEmailConsumer>();

            Console.WriteLine("Service running...");
        }

        public override void Stop()
        {
            Console.WriteLine("Service exiting...");

            _bus.Dispose();
        }
    }
}