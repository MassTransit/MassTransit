namespace MassTransit.AspNetCoreIntegration.HealthChecks
{
    public class HealthCheckOptions
    {
        public string BusHealthCheckName { get; set; }
        public string ReceiveEndpointHealthCheckName { get; set; }

        public static HealthCheckOptions Default
            => new HealthCheckOptions
            {
                BusHealthCheckName = "masstransit-bus",
                ReceiveEndpointHealthCheckName = "masstransit-endpoint"
            };
    }
}
