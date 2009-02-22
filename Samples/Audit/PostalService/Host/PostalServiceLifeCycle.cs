namespace PostalService.Host
{
	using System;
	using MassTransit;
	using Microsoft.Practices.ServiceLocation;

    public class PostalServiceLifeCycle
    {
        private IServiceBus _bus;

        public void Start()
        {
            _bus = ServiceLocator.Current.GetInstance<IServiceBus>("server");

            _bus.Subscribe<SendEmailConsumer>();

            Console.WriteLine("Service running...");
        }

        public void Stop()
        {
            Console.WriteLine("Service exiting...");

            _bus.Dispose();
        }
    }
}