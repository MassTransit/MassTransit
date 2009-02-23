namespace PostalService.Host
{
	using System;
	using MassTransit;
	using Microsoft.Practices.ServiceLocation;

    public class PostalService
    {
        private IServiceBus _bus;
        private UnsubscribeAction _unsubscribe;

        public void Start()
        {
            _bus = ServiceLocator.Current.GetInstance<IServiceBus>("server");

            _unsubscribe = _bus.Subscribe<SendEmailConsumer>();

            Console.WriteLine("Service running...");
        }

        public void Stop()
        {
            Console.WriteLine("Service exiting...");
            _unsubscribe();
            _bus.Dispose();
        }
    }
}