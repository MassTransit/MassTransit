namespace CodeCamp.Service
{
    using System;
    using Domain;
    using MassTransit;

    public class AuditService
    {
        private IServiceBus _bus;

        public void Start()
        {
            //TODO: Fix
            //_bus = ServiceLocator.Current.GetInstance<IServiceBus>("server");

            _bus.Subscribe<Responder>();
            _bus.Subscribe<RegisterUserSaga>();

            Console.WriteLine("Service running...");
        }

        public void Stop()
        {
            Console.WriteLine("Service exiting...");

            _bus.Dispose();
        }
    }
}