#nullable enable
namespace MassTransit;

public class SqlTransportOptions
{
    public string? Host { get; set; }
    public int? Port { get; set; }
    public string? Database { get; set; }
    public string? Schema { get; set; }
    public string? Role { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }

    public string? AdminUsername { get; set; }
    public string? AdminPassword { get; set; }

    /// <summary>
    /// Optional, if specified, will be parsed to capture additional properties on the connection.
    /// </summary>
    public string? ConnectionString { get; set; }
}
