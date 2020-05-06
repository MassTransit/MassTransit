namespace MassTransit.Monitoring.Health
{
    using System;
    using System.Collections.Generic;


    public readonly struct HealthResult
    {
        static readonly IReadOnlyDictionary<string, object> _emptyReadOnlyDictionary = new Dictionary<string, object>();

        HealthResult(BusHealthStatus status, string description = null, Exception exception = null,
            IReadOnlyDictionary<string, object> data = null)
        {
            Status = status;
            Description = description;
            Exception = exception;
            Data = data ?? _emptyReadOnlyDictionary;
        }

        public readonly IReadOnlyDictionary<string, object> Data;

        public readonly string Description;

        public readonly Exception Exception;

        public readonly BusHealthStatus Status;

        public static HealthResult Healthy(string description = null, IReadOnlyDictionary<string, object> data = null)
        {
            return new HealthResult(BusHealthStatus.Healthy, description, exception: null, data);
        }

        public static HealthResult Degraded(string description = null, Exception exception = null, IReadOnlyDictionary<string, object> data = null)
        {
            return new HealthResult(BusHealthStatus.Degraded, description, exception: null, data);
        }

        public static HealthResult Unhealthy(string description = null, Exception exception = null, IReadOnlyDictionary<string, object> data = null)
        {
            return new HealthResult(BusHealthStatus.Unhealthy, description, exception, data);
        }
    }
}
