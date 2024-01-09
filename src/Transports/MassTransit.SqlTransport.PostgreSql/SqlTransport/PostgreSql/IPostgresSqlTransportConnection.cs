namespace MassTransit.SqlTransport.PostgreSql
{
    using MassTransit.SqlTransport;
    using Npgsql;


    public interface IPostgresSqlTransportConnection :
        ISqlTransportConnection
    {
        new NpgsqlConnection Connection { get; }

        NpgsqlCommand CreateCommand(string commandText);
    }
}
