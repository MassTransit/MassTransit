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

using MassTransit.NHibernateIntegration.Tests.Framework;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace MassTransit.NHibernateIntegration.Tests.Sagas
{
	[TestFixture, Category("Integration")]
	public class SagaRepository_Specs
	{
		private NHibernate.Cfg.Configuration _cfg;

		[SetUp]
		public void Setup()
		{
			_cfg = TestConfigurator.CreateConfiguration(ConnectionString, null);
		}

		[Test, Explicit]
		public void First_we_need_a_schema_to_test()
		{
			new SchemaExport(_cfg).Create(true, true);
		}

		public virtual string ConnectionString
		{
			get { return "Data Source=:memory:;Pooling=True;Max Pool Size=1"; }
		}
	}
}