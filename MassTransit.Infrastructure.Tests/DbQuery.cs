namespace MassTransit.SubscriptionStorage.Tests
{
	using System.Data.SqlClient;

	public class DbQuery
	{
		private readonly string _connectionString;

		public DbQuery(string connectionString)
		{
			_connectionString = connectionString;
		}

		public object ExecuteScalar(string sql)
		{
			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				conn.Open();

				using (SqlCommand cmd = new SqlCommand(sql, conn))
				{
					return cmd.ExecuteScalar();
				}
			}
		}

		public int ExecuteCommand(string sql)
		{
			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				conn.Open();

				using (SqlCommand cmd = new SqlCommand(sql, conn))
				{
					return cmd.ExecuteNonQuery();
				}
			}
			
		}
	}
}