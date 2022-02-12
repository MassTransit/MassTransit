namespace MassTransit
{
    /// <summary>
    /// The host where an event or otherwise was produced
    /// a routing slip
    /// </summary>
    public interface HostInfo
    {
        /// <summary>
        /// The machine name (or role instance name) of the local machine
        /// </summary>
        string? MachineName { get; }

        /// <summary>
        /// The process name hosting the routing slip activity
        /// </summary>
        string? ProcessName { get; }

        /// <summary>
        /// The processId of the hosting process
        /// </summary>
        int ProcessId { get; }

        /// <summary>
        /// The assembly where the exception occurred
        /// </summary>
        string? Assembly { get; }

        /// <summary>
        /// The assembly version
        /// </summary>
        string? AssemblyVersion { get; }

        /// <summary>
        /// The .NET framework version
        /// </summary>
        string? FrameworkVersion { get; }

        /// <summary>
        /// The version of MassTransit used by the process
        /// </summary>
        string? MassTransitVersion { get; }

        /// <summary>
        /// The operating system version hosting the application
        /// </summary>
        string? OperatingSystemVersion { get; }
    }
}
