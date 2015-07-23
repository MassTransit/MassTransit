// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.AutomatonymousTests
{
    using System;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using EntityFrameworkIntegration;
    using Mehdime.Entity;


    public class SagaDbContextFactoryProvider :
        IDbContextFactory
    {
        /// <summary>
        /// This is a list of the connection strings that we will attempt to find what LocalDb versions
        /// are on the local pc which we can run the unit tests against
        /// </summary>
        private readonly string[] _possibleLocalDbConnectionStrings = new[]
        {
            @"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=True;Initial Catalog=MassTransitUnitTests_v12_2015;",  // the localdb installed with VS 2015
            @"Data Source=(LocalDb)\ProjectsV12;Integrated Security=True;Initial Catalog=MassTransitUnitTests_v12;",        // the localdb with VS 2013
            @"Data Source=(LocalDb)\v11.0;Integrated Security=True;Initial Catalog=MassTransitUnitTests_v11;"               // the older version of localdb
        };

        private static object _lockConnectionString = new object();
        private static string _connectionString;

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            TrySetLocalDbConnectionString();

            return new SagaDbContext<ShoppingChore, EntityFrameworkShoppingChoreMap>(_connectionString) as TDbContext;
        }

        /// <summary>
        /// Loops through the array of potential localdb connection strings to find one that we can use for the unit tests
        /// </summary>
        private void TrySetLocalDbConnectionString()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                lock (_lockConnectionString)
                {
                    if (string.IsNullOrWhiteSpace(_connectionString))
                    {
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
                }
            }
        }
    }
}