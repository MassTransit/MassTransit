namespace MassTransit.DapperIntegration.Tests
{
    using System;
#if NETCOREAPP
    using Microsoft.Data.SqlClient;
#else
        using System.Data.SqlClient;
#endif


    public static class LocalDbConnectionStringProvider
    {
        /// <summary>
        /// This is a list of the connection strings that we will attempt to find what LocalDb versions
        /// are on the local pc which we can run the unit tests against
        /// </summary>
        private static readonly string[] _possibleLocalDbConnectionStrings = new[]
        {
            @"Server=tcp:localhost;Persist Security Info=False;User ID=sa;Password=Password12!;Encrypt=False;TrustServerCertificate=True;", // the linux mssql 2017 installed on appveyor
            @"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=True;Initial Catalog=MassTransitUnitTests_v12_2015;", // the localdb installed with VS 2015
            @"Data Source=(LocalDb)\ProjectsV12;Integrated Security=True;Initial Catalog=MassTransitUnitTests_v12;", // the localdb with VS 2013
            @"Data Source=(LocalDb)\v11.0;Integrated Security=True;Initial Catalog=MassTransitUnitTests_v11;" // the older version of localdb
        };

        private static object _lockConnectionString = new object();
        private static string _connectionString;

        /// <summary>
        /// Loops through the array of potential localdb connection strings to find one that we can use for the unit tests
        /// </summary>
        public static string GetLocalDbConnectionString()
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
                return _connectionString;

            lock (_lockConnectionString)
            {
                if (!string.IsNullOrWhiteSpace(_connectionString))
                    return _connectionString;

                // Lets find a localdb that we can use for our unit test
                foreach (var connectionString in _possibleLocalDbConnectionStrings)
                {
                    try
                    {
                        using (var connection = new SqlConnection(connectionString))
                        {
                            // It worked, we can save this as our connection string
                            _connectionString = connectionString;
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        // Swallow
                    }
                }

                // If we looped through all possible localdb connection strings and didn't find one, we fail.
                if (string.IsNullOrWhiteSpace(_connectionString))
                    throw new InvalidOperationException(
                        "Couldn't connect to any of the LocalDB Databases. You might have a version installed that is not in the list. Please check the list and modify as necessary");
            }

            return _connectionString;
        }
    }
}
