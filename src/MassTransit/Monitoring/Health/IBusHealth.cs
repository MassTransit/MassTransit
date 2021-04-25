namespace MassTransit.Monitoring.Health
{
    using System;


    [Obsolete("Bus health can now be checked on the bus itself, via IBusControl")]
    public interface IBusHealth
    {
        string Name { get; }

        HealthResult CheckHealth();
    }
}
