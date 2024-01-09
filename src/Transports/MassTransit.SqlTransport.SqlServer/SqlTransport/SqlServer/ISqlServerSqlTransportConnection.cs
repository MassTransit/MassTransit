namespace MassTransit.SqlTransport.SqlServer
{
    using MassTransit.SqlTransport;
    using Microsoft.Data.SqlClient;


    public interface ISqlServerSqlTransportConnection :
        ISqlTransportConnection
    {
        new SqlConnection Connection { get; }

        SqlCommand CreateCommand(string commandText);
    }
}
