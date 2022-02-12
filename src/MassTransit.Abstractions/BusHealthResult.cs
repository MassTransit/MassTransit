namespace MassTransit
{
    using System;
    using System.Collections.Generic;


    public class BusHealthResult
    {
        public readonly IReadOnlyDictionary<string, EndpointHealthResult> Endpoints;

        public readonly Exception? Exception;

        public readonly BusHealthStatus Status;

        BusHealthResult(BusHealthStatus status, string description, Exception? exception, IReadOnlyDictionary<string, EndpointHealthResult> endpoints)
        {
            Status = status;
            Description = description;
            Exception = exception;
            Endpoints = endpoints;
        }

        public string Description { get; }

        public static BusHealthResult Healthy(string description, IReadOnlyDictionary<string, EndpointHealthResult> endpoints)
        {
            return new BusHealthResult(BusHealthStatus.Healthy, description, null, endpoints);
        }

        public static BusHealthResult Degraded(string description, Exception exception, IReadOnlyDictionary<string, EndpointHealthResult> endpoints)
        {
            return new BusHealthResult(BusHealthStatus.Degraded, description, exception, endpoints);
        }

        public static BusHealthResult Unhealthy(string description, Exception exception, IReadOnlyDictionary<string, EndpointHealthResult> endpoints)
        {
            return new BusHealthResult(BusHealthStatus.Unhealthy, description, exception, endpoints);
        }
    }
}
