namespace MassTransit.NHibernateIntegration.Tests
{
    using Microsoft.Data.SqlClient;


    public class DbQuery
    {
        readonly string _connectionString;

        public DbQuery(string connectionString)
        {
            _connectionString = connectionString;
        }

        public object ExecuteScalar(string sql)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand(sql, conn))
                {
                    return cmd.ExecuteScalar();
                }
            }
        }

        public int ExecuteCommand(string sql)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand(sql, conn))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
