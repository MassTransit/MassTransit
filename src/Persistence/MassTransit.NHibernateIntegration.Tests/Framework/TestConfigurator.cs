// Copyright 2012 Henrik Feldt
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

using System;
using System.Data.SQLite;
using MassTransit.NHibernateIntegration.Saga;
using MassTransit.NHibernateIntegration.Tests.Sagas;
using MassTransit.Tests.Saga.StateMachine;
using MassTransit.Util;
using NHibernate.Cache;
using NHibernate.Dialect;
using NHibernate.Driver;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace MassTransit.NHibernateIntegration.Tests.Framework
{
	public static class TestConfigurator
	{
		public static NHibernate.Cfg.Configuration CreateConfiguration([CanBeNull] string connectionString, [CanBeNull] Action<NHibernate.Cfg.Configuration> configurator)
		{
			Assert.NotNull(typeof(SQLiteConnection));

			connectionString = connectionString ?? "Data Source=:memory:";

			var cfg = new NHibernate.Cfg.Configuration()
				.SetProperty(Environment.ConnectionDriver, typeof (SQLite20Driver).AssemblyQualifiedName)
				.SetProperty(Environment.Dialect, typeof (SQLiteDialect).AssemblyQualifiedName)
				//.SetProperty(Environment.ConnectionDriver, typeof(Sql2008ClientDriver).AssemblyQualifiedName)
				//.SetProperty(Environment.Dialect, typeof(MsSql2008Dialect).AssemblyQualifiedName)
				.SetProperty(Environment.ConnectionString, connectionString)
				//.SetProperty(Environment.ProxyFactoryFactoryClass, typeof(ProxyFactoryFactory).AssemblyQualifiedName)
				//.SetProperty(Environment.ReleaseConnections, "never")
				.SetProperty(Environment.UseSecondLevelCache, "true")
				.SetProperty(Environment.UseQueryCache, "true")
				.SetProperty(Environment.CacheProvider, typeof (HashtableCacheProvider).AssemblyQualifiedName)
				//.SetProperty(Environment.DefaultSchema, "bus")
				.AddAssembly(typeof (RegisterUserStateMachine).Assembly)
				.AddAssembly(typeof (SagaRepository_Specs).Assembly);

			if (configurator != null)
				configurator(cfg);

			return cfg;
		}
	}
}