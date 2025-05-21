namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.SqlClient;


    public static class LocalDbConnectionStringProvider
    {
        /// <summary>
        /// This is a list of the connection strings that we will attempt to find what LocalDb versions
        /// are on the local pc which we can run the unit tests against
        /// </summary>
        static readonly string[] _possibleLocalDbConnectionStrings =
        {
            @"Server=tcp:localhost;Persist Security Info=False;User ID=sa;Password=Password12!;Encrypt=False;TrustServerCertificate=True", // the linux mssql 2017 installed on appveyor
            @"Server=tcp:mssql;Persist Security Info=False;User ID=sa;Password=Password12!;Encrypt=False;TrustServerCertificate=True", // the linux mssql 2017 installed on gha
            @"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=True", // the localdb installed with VS 2015
            @"Data Source=(LocalDb)\ProjectsV12;Integrated Security=True", // the localdb with VS 2013
            @"Data Source=(LocalDb)\v11.0;Integrated Security=True" // the older version of localdb
        };

        static readonly object _lockConnectionString = new object();
        static string _connectionString;

        /// <summary>
        /// Loops through the array of potential localdb connection strings to find one that we can use for the unit tests
        /// </summary>
        public static string GetLocalDbConnectionString(string initialCatalog = "MassTransitUnitTests_v12_2015")
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
                return _connectionString;

            lock (_lockConnectionString)
            {
                if (!string.IsNullOrWhiteSpace(_connectionString))
                    return _connectionString;

                // Lets find a localdb that we can use for our unit test
                _connectionString = TestConnections(_possibleLocalDbConnectionStrings, initialCatalog);
            }

            return _connectionString;

            static string TestConnections(IEnumerable<string> connectionStrings, string initialCatalog)
            {
                var exceptions = new List<Exception>
                {
                    new InvalidOperationException(
                        "Couldn't connect to any of the LocalDB Databases. You might have a version installed that is not in the list. Please check the list and modify as necessary")
                };

                foreach (var candidate in connectionStrings)
                {
                    try
                    {
                        using var connection = new SqlConnection(candidate);
                        connection.Open();

                        EnsureDatabaseExists(connection, initialCatalog);

                        // It worked, we can save this as our connection string.
                        return $"{candidate}; Initial Catalog={initialCatalog}";
                    }
                    catch (Exception ex)
                    {
                        // Swallow
                        exceptions.Add(ex);
                    }
                }
                
                throw new AggregateException(exceptions);
            }
        }

        static void EnsureDatabaseExists(SqlConnection connection, string database)
        {
            // Check that the database itself actually exists
            using var command = connection.CreateCommand();

            command.CommandText = "SELECT COUNT(*) FROM sys.databases WHERE name = @name;";
            command.Parameters.AddWithValue("@name", database);

            var exists = (int) command.ExecuteScalar() > 0;
            if (exists)
                return;

            // Create the missing database now
            command.CommandText = $"CREATE DATABASE [{database}];";
            command.Parameters.Clear();

            command.ExecuteNonQuery();
        }
    }
}
