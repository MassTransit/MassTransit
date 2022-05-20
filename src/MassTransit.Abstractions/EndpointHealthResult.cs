namespace MassTransit
{
    using System;


    public readonly struct EndpointHealthResult
    {
        EndpointHealthResult(BusHealthStatus status, IReceiveEndpoint receiveEndpoint, string? description = null, Exception? exception = null)
        {
            Status = status;
            ReceiveEndpoint = receiveEndpoint;
            Description = description;
            Exception = exception;
        }

        public readonly BusHealthStatus Status;

        public readonly string? Description;

        public readonly Exception? Exception;

        public readonly IReceiveEndpoint ReceiveEndpoint;

        public static EndpointHealthResult Healthy(IReceiveEndpoint receiveEndpoint, string? description)
        {
            return new EndpointHealthResult(BusHealthStatus.Healthy, receiveEndpoint, description);
        }

        public static EndpointHealthResult Degraded(IReceiveEndpoint receiveEndpoint, string? description)
        {
            return new EndpointHealthResult(BusHealthStatus.Degraded, receiveEndpoint, description);
        }

        public static EndpointHealthResult Unhealthy(IReceiveEndpoint receiveEndpoint, string? description, Exception? exception)
        {
            return new EndpointHealthResult(BusHealthStatus.Unhealthy, receiveEndpoint, description, exception);
        }
    }
}
