namespace MassTransit.QuartzService.Configuration
{
    using System.Data;


    public interface IConnectionProvider
    {
        IDbConnection GetConnection();

        /// <summary>
        /// Get a command object for executing a query against a database
        /// </summary>
        /// <param name="sql">The SQL string</param>
        /// <returns>An owned command object</returns>
        IDbCommand GetCommand(string sql);
    }
}