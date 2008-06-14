namespace MassTransit.ServiceBus.HealthMonitoring
{
    using System;
    using System.Collections.Generic;

    public interface IHealthCache
    {
        void Add(HealthInformation information);
        IList<HealthInformation> List();
        HealthInformation Get(Uri uri);
        void Update(HealthInformation information);
    }
}