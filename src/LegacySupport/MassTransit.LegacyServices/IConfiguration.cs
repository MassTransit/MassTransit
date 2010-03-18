namespace LegacyRuntime
{
    using System;

    public interface IConfiguration
    {
        bool LegacyServiceEnabled { get; }
        Uri LegacyServiceControlUri { get; }
        Uri LegacyServiceDataUri { get; }

        Uri SubscriptionServiceUri { get; }
    }
}