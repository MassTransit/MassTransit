namespace MassTransit.Services.HealthMonitoring.Messages
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class HealthUpdate
    {
        public HealthUpdate()
        {
            Information = new List<HealthInformation>();
        }

        public IList<HealthInformation> Information { get; set; }
    }
}