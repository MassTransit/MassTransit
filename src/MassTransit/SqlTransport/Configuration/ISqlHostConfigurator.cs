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

    string? Host { set; }
    int? Port { set; }
    string? Database { set; }
    string? Schema { set; }
    string? Username { set; }
    string? Password { set; }

    string? VirtualHost { set; }
    string? Area { set; }

    /// <summary>
    /// Sets the isolation level used for database transactions (default: Repeatable Read)
    /// </summary>
    IsolationLevel IsolationLevel { set; }

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
