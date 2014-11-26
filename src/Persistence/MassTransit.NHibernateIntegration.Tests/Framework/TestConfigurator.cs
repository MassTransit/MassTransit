// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.NHibernateIntegration.Tests.Framework
{
    using System;
    using System.Data.SQLite;
    using NHibernate.Cache;
    using NHibernate.Cfg;
    using NHibernate.Dialect;
    using NHibernate.Driver;
    using NUnit.Framework;
    using Sagas;


    public static class TestConfigurator
    {
        public static Configuration CreateConfiguration(string connectionString, Action<Configuration> configurator)
        {
            Assert.NotNull(typeof(SQLiteConnection));

            connectionString = connectionString ?? "Data Source=:memory:";

            Configuration cfg = new Configuration()
                .SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, typeof(SQLite20Driver).AssemblyQualifiedName)
                .SetProperty(NHibernate.Cfg.Environment.Dialect, typeof(SQLiteDialect).AssemblyQualifiedName)
                //.SetProperty(Environment.ConnectionDriver, typeof(Sql2008ClientDriver).AssemblyQualifiedName)
                //.SetProperty(Environment.Dialect, typeof(MsSql2008Dialect).AssemblyQualifiedName)
                .SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString)
                //.SetProperty(Environment.ProxyFactoryFactoryClass, typeof(ProxyFactoryFactory).AssemblyQualifiedName)
                //.SetProperty(Environment.ReleaseConnections, "never")
                .SetProperty(NHibernate.Cfg.Environment.UseSecondLevelCache, "true")
                .SetProperty(NHibernate.Cfg.Environment.UseQueryCache, "true")
                .SetProperty(NHibernate.Cfg.Environment.CacheProvider, typeof(HashtableCacheProvider).AssemblyQualifiedName)
                //.SetProperty(Environment.DefaultSchema, "bus")
                .AddAssembly(typeof(SagaRepository_Specs).Assembly);

            if (configurator != null)
                configurator(cfg);

            return cfg;
        }
    }
}