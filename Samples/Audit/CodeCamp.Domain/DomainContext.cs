namespace CodeCamp.Domain
{
    using Core;
    using Magnum.Common.ObjectExtensions;
    using MassTransit.ServiceBus;

    public static class DomainContext
    {
        private static IServiceBus _serviceBus;
        private static IRepository<User> _userRepository;

        public static IRepository<User> UserRepository
        {
            get { return _userRepository; }
        }

        public static void Publish<T>(T message) where T : class
        {
            _serviceBus.Publish(message);
        }

        public static void Initialize(IServiceBus bus)
        {
            bus.MustNotBeNull();

            _serviceBus = bus;
        }
    }
}