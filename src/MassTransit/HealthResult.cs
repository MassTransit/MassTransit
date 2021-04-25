namespace MassTransit
{
    using System;
    using System.Collections.Generic;


    public class HealthResult
    {
        public readonly IReadOnlyDictionary<string, EndpointHealthResult> Endpoints;

        public readonly Exception Exception;

        public readonly BusHealthStatus Status;

        HealthResult(BusHealthStatus status, string description, Exception exception, IReadOnlyDictionary<string, EndpointHealthResult> endpoints)
        {
            Status = status;
            Description = description;
            Exception = exception;
            Endpoints = endpoints;
        }

        public string Description { get; }

        public static HealthResult Healthy(string description, IReadOnlyDictionary<string, EndpointHealthResult> endpoints)
        {
            return new HealthResult(BusHealthStatus.Healthy, description, null, endpoints);
        }

        public static HealthResult Degraded(string description, Exception exception, IReadOnlyDictionary<string, EndpointHealthResult> endpoints)
        {
            return new HealthResult(BusHealthStatus.Degraded, description, exception, endpoints);
        }

        public static HealthResult Unhealthy(string description, Exception exception, IReadOnlyDictionary<string, EndpointHealthResult> endpoints)
        {
            return new HealthResult(BusHealthStatus.Unhealthy, description, exception, endpoints);
        }
    }
}
