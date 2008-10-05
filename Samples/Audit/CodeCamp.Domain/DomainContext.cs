namespace CodeCamp.Domain
{
    using Magnum.Common.ObjectExtensions;
    using MassTransit.ServiceBus;
    using Microsoft.Practices.ServiceLocation;

    public static class DomainContext
    {
        private static IServiceBus _serviceBus;
        private static IServiceLocator _serviceLocator;

        public static IServiceLocator ServiceLocator
        {
            get { return _serviceLocator; }
        }

        public static void Publish<T>(T message) where T : class
        {
            _serviceBus.Publish(message);
        }

        public static void Initialize(IServiceBus bus, IServiceLocator serviceLocator)
        {
            bus.MustNotBeNull();
            serviceLocator.MustNotBeNull();

            _serviceBus = bus;
            _serviceLocator = serviceLocator;
        }
    }
}