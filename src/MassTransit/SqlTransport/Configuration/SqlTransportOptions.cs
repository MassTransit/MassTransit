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

    /// <summary>
    /// If specified, changes the connection limit from the default value (10)
    /// </summary>
    public int? ConnectionLimit { get; set; }

    public AuthenticationMode AuthenticationMode { get; set; }

    public TokenIssuerOptions? TokenIssuerOptions { get; set; }
}


public class TokenIssuerOptions
{
    public TokenIssuer Issuer { get; set; }

    public string? AwsRdsRegionSystemName { get; set; }
}


public enum TokenIssuer
{
    AwsRds
}

public enum AuthenticationMode
{
    Password,
    Token
}
