#nullable enable
namespace MassTransit.JobService
{
    using System;
    using Configuration;


    /// <summary>
    /// Settings relevant to the job consumer endpoints and the service instance
    /// </summary>
    public interface JobServiceSettings :
        IOptions
    {
        IJobService JobService { get; }

        TimeSpan HeartbeatInterval { get; }

        Uri? InstanceAddress { get; }

        IReceiveEndpointConfigurator? InstanceEndpointConfigurator { get; }
    }
}
