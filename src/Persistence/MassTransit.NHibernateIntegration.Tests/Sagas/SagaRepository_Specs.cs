// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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

using System.Data.SQLite;
using NHibernate.Cache;
using NHibernate.Dialect;
using NHibernate.Driver;

namespace MassTransit.NHibernateIntegration.Tests.Sagas
{
    using MassTransit.Tests.Saga.StateMachine;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;
    using NUnit.Framework;
    using Saga;

    [TestFixture, Category("Integration")]
    public class SagaRepository_Specs
    {
    	Configuration _cfg;

    	[SetUp]
        public void Setup()
        {
			Assert.NotNull(typeof(SQLiteConnection));

			_cfg = new Configuration()
				.SetProperty(Environment.ConnectionDriver, typeof(SQLite20Driver).AssemblyQualifiedName)
				.SetProperty(Environment.Dialect, typeof(SQLiteDialect).AssemblyQualifiedName)
					//.SetProperty(Environment.ConnectionDriver, typeof(Sql2008ClientDriver).AssemblyQualifiedName)
					//.SetProperty(Environment.Dialect, typeof(MsSql2008Dialect).AssemblyQualifiedName)
				.SetProperty(Environment.ConnectionString, ConnectionString)
					//.SetProperty(Environment.ProxyFactoryFactoryClass, typeof(ProxyFactoryFactory).AssemblyQualifiedName)
				.SetProperty(Environment.ReleaseConnections, "on_close")
				.SetProperty(Environment.UseSecondLevelCache, "true")
				.SetProperty(Environment.UseQueryCache, "true")
				.SetProperty(Environment.CacheProvider, typeof(HashtableCacheProvider).AssemblyQualifiedName)
				.SetProperty(Environment.DefaultSchema, "bus")
				.AddAssembly(typeof (NHibernateSagaRepository<>).Assembly)
				.AddAssembly(typeof (RegisterUserStateMachine).Assembly)
				.AddAssembly(typeof (SagaRepository_Specs).Assembly);
        }

    	[Test, Explicit]
        public void First_we_need_a_schema_to_test()
        {
            new SchemaExport(_cfg).Create(true, true);
        }

		public virtual string ConnectionString
		{
			get { return "Data Source=:memory:"; }
		}
    }
}