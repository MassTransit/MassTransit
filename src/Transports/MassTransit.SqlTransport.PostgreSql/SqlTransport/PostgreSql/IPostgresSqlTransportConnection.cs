namespace MassTransit.SqlTransport.PostgreSql
{
    using Npgsql;


    public interface IPostgresSqlTransportConnection :
        ISqlTransportConnection
    {
        new NpgsqlConnection Connection { get; }

        NpgsqlCommand CreateCommand(string commandText);
    }
}
