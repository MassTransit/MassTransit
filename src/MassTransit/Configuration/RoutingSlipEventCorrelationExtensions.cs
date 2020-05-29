namespace MassTransit
{
    using Courier;


    public static class RoutingSlipEventCorrelationExtensions
    {
        public static void ConfigureRoutingSlipEventCorrelation(this IBusFactoryConfigurator configurator)
        {
            RoutingSlipEventCorrelation.ConfigureCorrelationIds();
        }
    }
}
