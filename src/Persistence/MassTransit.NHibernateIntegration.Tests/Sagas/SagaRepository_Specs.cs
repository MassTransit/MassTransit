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
        [SetUp]
        public void Setup()
        {
            _cfg = new Configuration();

            _cfg.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            _cfg.SetProperty("connection.driver_class", "NHibernate.Driver.SqlClientDriver");
            _cfg.SetProperty("connection.connection_string", _connectionString);
            _cfg.SetProperty("dialect", "NHibernate.Dialect.MsSql2005Dialect");
            _cfg.SetProperty("default_schema", "bus");

            _cfg.AddAssembly(typeof (NHibernateSagaRepository<>).Assembly);
            _cfg.AddAssembly(typeof (RegisterUserStateMachine).Assembly);
            _cfg.AddAssembly(typeof (SagaRepository_Specs).Assembly);
        }

        const string _connectionString = "Server=localhost;initial catalog=test;Trusted_Connection=yes";
        Configuration _cfg;

        [Test, Explicit]
        public void First_we_need_a_schema_to_test()
        {
            new SchemaExport(_cfg).Create(true, true);
        }
    }
}