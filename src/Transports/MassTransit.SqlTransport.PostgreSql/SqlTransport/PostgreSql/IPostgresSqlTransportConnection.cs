namespace MassTransit.SqlTransport.PostgreSql;

using Npgsql;


public interface IPostgresSqlTransportConnection :
    ISqlTransportConnection
{
    NpgsqlConnection Connection { get; }

    NpgsqlCommand CreateCommand(string commandText);
}
