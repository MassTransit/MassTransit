namespace MassTransit.ServiceBus.Configuration
{
    public static class BusBuilder
    {
        private static IObjectBuilder _objectBuilder;

        public static void SetObjectBuilder(IObjectBuilder builder)
        {
            _objectBuilder = builder;
        }

        public static ConfigureTheBus WithName(string name)
        {
            return new ConfigureTheBus(new BusOptions(_objectBuilder){Name = name});
        }
    }
}