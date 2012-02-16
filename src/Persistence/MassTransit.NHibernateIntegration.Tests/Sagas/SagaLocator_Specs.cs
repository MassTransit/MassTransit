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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.Extensions;
    using MassTransit.Saga;
    using MassTransit.Tests;
    using MassTransit.Tests.Messages;
    using MassTransit.Tests.Saga;
    using MassTransit.Tests.Saga.Locator;
    using MassTransit.Tests.Saga.StateMachine;
    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;
    using NUnit.Framework;
    using Saga;

    [TestFixture, Category("Integration")]
    public class When_using_the_saga_locator_with_NHibernate
    {
        [SetUp]
        public void Setup()
        {
            _cfg = new Configuration();

            _cfg.SetProperty("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            _cfg.SetProperty("connection.driver_class", "NHibernate.Driver.SqlClientDriver");
            _cfg.SetProperty("connection.connection_string", _connectionString);
            _cfg.SetProperty("dialect", "NHibernate.Dialect.MsSql2005Dialect");
            _cfg.SetProperty("show_sql", "true");

            _cfg.AddAssembly(typeof (NHibernateSagaRepository<>).Assembly);
            _cfg.AddAssembly(typeof (RegisterUserStateMachine).Assembly);
            _cfg.AddAssembly(typeof (When_using_the_saga_locator_with_NHibernate).Assembly);

            _sessionFactory = _cfg.BuildSessionFactory();

            _sagaId = NewId.NextGuid();
        }

        Guid _sagaId;

        const string _connectionString = "Server=localhost;initial catalog=test;Trusted_Connection=yes";
        Configuration _cfg;
        ISessionFactory _sessionFactory;


        IEnumerable<Action<IConsumeContext<InitiateSimpleSaga>>> GetHandlers(TestSaga instance,
                                                                             IConsumeContext<InitiateSimpleSaga> context)
        {
            yield return x => instance.RaiseEvent(TestSaga.Initiate, x.Message);
        }

        [Test]
        public void A_correlated_message_should_find_the_correct_saga()
        {
            var repository = new NHibernateSagaRepository<TestSaga>(_sessionFactory);
            var ping = new PingMessage(_sagaId);

            var initiatePolicy = new InitiatingSagaPolicy<TestSaga, InitiateSimpleSaga>(x => x.CorrelationId, x => false);

            var message = new InitiateSimpleSaga(_sagaId);
            IConsumeContext<InitiateSimpleSaga> context = message.ToConsumeContext();

            repository.GetSaga(context, message.CorrelationId, GetHandlers, initiatePolicy)
                .Each(x => x(context));

            List<TestSaga> sagas = repository.Where(x => x.CorrelationId == _sagaId).ToList();

            Assert.AreEqual(1, sagas.Count);
            Assert.IsNotNull(sagas[0]);
            Assert.AreEqual(_sagaId, sagas[0].CorrelationId);
        }

        [Test, Explicit]
        public void First_we_need_a_schema_to_test()
        {
            var schemaExport = new SchemaExport(_cfg);
            schemaExport.Drop(true, true);
            schemaExport.Create(true, true);
        }
    }
}