namespace PostalService.Host
{
	using System;
	using MassTransit;

    public class PostalService
    {
        private IServiceBus _bus;
        private UnsubscribeAction _unsubscribe;

        public void Start()
        {
            //TODO: fix this
            //_bus = ServiceLocator.Current.GetInstance<IServiceBus>("server");

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