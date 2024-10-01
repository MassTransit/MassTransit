namespace MassTransit.SqlTransport.SqlServer;

using Microsoft.Data.SqlClient;


public interface ISqlServerSqlTransportConnection :
    ISqlTransportConnection
{
    SqlConnection Connection { get; }
}
