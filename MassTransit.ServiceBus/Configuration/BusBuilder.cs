namespace MassTransit.ServiceBus.Configuration
{
    public static class BusBuilder
    {
        public static ConfigureTheBus WithName(string name)
        {
            return new ConfigureTheBus(new BusOptions{Name = name});
        }
    }
}