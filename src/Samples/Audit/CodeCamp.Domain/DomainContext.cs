namespace CodeCamp.Domain
{
    using Magnum;
    using Magnum.Extensions;
    using MassTransit;

    public static class DomainContext
    {
        private static IServiceBus _serviceBus;
        private static IObjectBuilder _serviceLocator;

        public static IObjectBuilder ServiceLocator
        {
            get { return _serviceLocator; }
        }

        public static void Publish<T>(T message) where T : class
        {
            _serviceBus.Publish(message);
        }

        public static void Initialize(IServiceBus bus, IObjectBuilder serviceLocator)
        {
            Guard.AgainstNull(bus, "bus","Must not be null");
            Guard.AgainstNull(serviceLocator, "serviceLocator");

            _serviceBus = bus;
            _serviceLocator = serviceLocator;
        }
    }
}