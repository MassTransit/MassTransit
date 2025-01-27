#nullable enable
namespace MassTransit;

using System;
using System.Data;


public interface ISqlHostConfigurator
{
    /// <summary>
    /// Set the connection string, which the underlying database provider will parse into its individual components and rebuild at runtime
    /// </summary>
    string? ConnectionString { set; }

    /// <summary>
    /// Optional, specifies a connection tag used to identify the connection in the database
    /// </summary>
    string? ConnectionTag { set; }

    /// <summary>
    /// The database server host name. If using SQL server with an instance, set the <see cref="InstanceName" /> separately.
    /// </summary>
    string? Host { set; }

    /// <summary>
    /// The instance name if using SQL Server instances
    /// </summary>
    string? InstanceName { set; }

    /// <summary>
    /// Optional, only specify if a custom port is being used.
    /// </summary>
    int? Port { set; }

    /// <summary>
    /// The database name
    /// </summary>
    string? Database { set; }

    /// <summary>
    /// The schema to use for the transport
    /// </summary>
    string? Schema { set; }

    /// <summary>
    /// The username for the bus to access the transport
    /// </summary>
    string? Username { set; }

    /// <summary>
    /// The password for the username
    /// </summary>
    string? Password { set; }

    string? VirtualHost { set; }

    string? Area { set; }

    /// <summary>
    /// Sets the isolation level used for database transactions (default: Repeatable Read)
    /// </summary>
    IsolationLevel IsolationLevel { set; }

    /// <summary>
    /// Sets the maximum number of connections used by the SQL transport concurrently.
    /// </summary>
    int ConnectionLimit { set; }

    /// <summary>
    /// Should typically be left to the default (true), reserved for use cases such as delegating maintenance activities explicitly as application quantities grow.
    /// </summary>
    bool MaintenanceEnabled { set; }

    /// <summary>
    /// How often database maintenance should be performed (metrics consolidation, topology cleanup, etc.)
    /// </summary>
    TimeSpan MaintenanceInterval { set; }

    /// <summary>
    /// How many metrics events to compute in each batch
    /// </summary>
    int MaintenanceBatchSize { set; }

    /// <summary>
    /// How often to purge auto-delete queues from the topology and all expired messages
    /// </summary>
    TimeSpan QueueCleanupInterval { set; }

    /// <summary>
    /// Specify the license text to use
    /// </summary>
    /// <param name="license">The license text</param>
    void UseLicense(string license);

    /// <summary>
    /// Specify the path to the file containing the license text
    /// </summary>
    /// <param name="path">The path to the file</param>
    void UseLicenseFile(string path);
}
