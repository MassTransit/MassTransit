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
using MassTransit.NHibernateIntegration.Tests.Framework;

namespace MassTransit.NHibernateIntegration.Tests.Sagas
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Magnum.Extensions;
    using MassTransit.Saga;
    using MassTransit.Tests.TextFixtures;
    using NHibernate;
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;
    using NUnit.Framework;
    using Saga;
    using TestFramework;
    using log4net;

    [TestFixture, Category("Integration")]
    public abstract class ConcurrentSagaTestFixtureBase :
        LoopbackTestFixture
    {
        private IDbConnection _openConnection;
        protected ISessionFactory SessionFactory;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            var provider = new NHibernateSessionFactoryProvider(new Type[]
                {
                    typeof(ConcurrentSagaMap), typeof(ConcurrentLegacySagaMap)
                });

            var sessionFactory = provider.GetSessionFactory();

            _openConnection = new SQLiteConnection(provider.Configuration.Properties[NHibernate.Cfg.Environment.ConnectionString]);
            _openConnection.Open();
            sessionFactory.OpenSession(_openConnection);

            SessionFactory = new SingleConnectionSessionFactory(sessionFactory, _openConnection);

            BuildSchema(provider.Configuration, _openConnection);
        }

        protected override void TeardownContext()
        {
            base.TeardownContext();

            if (_openConnection != null)
            {
                _openConnection.Close();
                _openConnection.Dispose();
            }

            if (SessionFactory != null)
                SessionFactory.Dispose();
        }

        static void BuildSchema(Configuration config, IDbConnection connection)
        {
            new SchemaExport(config).Execute(true, true, false, connection, null);
        }
    }

    [TestFixture, Category("Integration")]
    public class Sending_multiple_messages_to_the_same_saga_at_the_same_time :
        ConcurrentSagaTestFixtureBase
    {
        ISagaRepository<ConcurrentSaga> _sagaRepository;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _sagaRepository = new NHibernateSagaRepository<ConcurrentSaga>(SessionFactory);
        }

        [Test]
        public void Should_process_the_messages_in_order_and_not_at_the_same_time()
        {
            UnsubscribeAction unsubscribeAction = LocalBus.SubscribeSaga(_sagaRepository);

            Guid transactionId = NewId.NextGuid();

            Trace.WriteLine("Creating transaction for " + transactionId);

            int startValue = 1;

            var startConcurrentSaga = new StartConcurrentSaga
                {CorrelationId = transactionId, Name = "Chris", Value = startValue};

            LocalBus.Publish(startConcurrentSaga);

            var saga = _sagaRepository.ShouldContainSaga(transactionId, 8.Seconds());
            Assert.IsNotNull(saga);

            int nextValue = 2;
            var continueConcurrentSaga = new ContinueConcurrentSaga {CorrelationId = transactionId, Value = nextValue};

            LocalBus.Publish(continueConcurrentSaga);

            saga = _sagaRepository.ShouldContainSaga(x => x.CorrelationId == transactionId && x.Value == nextValue, 8.Seconds());
            Assert.IsNotNull(saga);

            unsubscribeAction();

            Assert.AreEqual(nextValue, saga.Value);
        }
    }

    [TestFixture, Category("Integration")]
    public class Sending_multiple_messages_to_the_same_saga_legacy_at_the_same_time :
        ConcurrentSagaTestFixtureBase
    {
        static readonly ILog _log =
            LogManager.GetLogger(typeof (Sending_multiple_messages_to_the_same_saga_legacy_at_the_same_time));

        ISagaRepository<ConcurrentLegacySaga> _sagaRepository;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _sagaRepository = new NHibernateSagaRepository<ConcurrentLegacySaga>(SessionFactory);
        }

        [Test]
        public void Should_process_the_messages_in_order_and_not_at_the_same_time()
        {
            UnsubscribeAction unsubscribeAction = LocalBus.SubscribeSaga(_sagaRepository);

            Guid transactionId = NewId.NextGuid();

            _log.Info("Creating transaction for " + transactionId);

            const int startValue = 1;

            var startConcurrentSaga = new StartConcurrentSaga
                {CorrelationId = transactionId, Name = "Chris", Value = startValue};

            LocalBus.Publish(startConcurrentSaga);
            _log.Info("Just published the start message");

            Thread.Sleep(500);

            const int nextValue = 2;
            var continueConcurrentSaga = new ContinueConcurrentSaga {CorrelationId = transactionId, Value = nextValue};

            LocalBus.Publish(continueConcurrentSaga);
            _log.Info("Just published the continue message");
            Thread.Sleep(8000);

            unsubscribeAction();
            foreach (ConcurrentLegacySaga saga in _sagaRepository.Where(x => true))
            {
                _log.Info("Found saga: " + saga.CorrelationId);
            }

            int currentValue = _sagaRepository.Where(x => x.CorrelationId == transactionId).First().Value;

            Assert.AreEqual(nextValue, currentValue);
        }
    }

    [TestFixture, Category("Integration")]
    public class Sending_multiple_initiating_messages_should_not_fail_badly :
        ConcurrentSagaTestFixtureBase
    {
        static readonly ILog _log =
            LogManager.GetLogger(typeof (Sending_multiple_initiating_messages_should_not_fail_badly));

        ISagaRepository<ConcurrentLegacySaga> _sagaRepository;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _sagaRepository = new NHibernateSagaRepository<ConcurrentLegacySaga>(SessionFactory);
        }

        [Test]
        public void Should_process_the_messages_in_order_and_not_at_the_same_time()
        {
            Guid transactionId = NewId.NextGuid();

            _log.Info("Creating transaction for " + transactionId);

            const int startValue = 1;

            var startConcurrentSaga = new StartConcurrentSaga
                {CorrelationId = transactionId, Name = "Chris", Value = startValue};

            LocalBus.Endpoint.Send(startConcurrentSaga);
            LocalBus.Endpoint.Send(startConcurrentSaga);

            _log.Info("Just published the start message");

            UnsubscribeAction unsubscribeAction = LocalBus.SubscribeSaga(_sagaRepository);

            Thread.Sleep(1500);

            const int nextValue = 2;
            var continueConcurrentSaga = new ContinueConcurrentSaga {CorrelationId = transactionId, Value = nextValue};

            LocalBus.Publish(continueConcurrentSaga);
            _log.Info("Just published the continue message");
            Thread.Sleep(8000);

            unsubscribeAction();
            foreach (ConcurrentLegacySaga saga in _sagaRepository.Where(x => true))
            {
                _log.Info("Found saga: " + saga.CorrelationId);
            }

            int currentValue = _sagaRepository.Where(x => x.CorrelationId == transactionId).First().Value;

            Assert.AreEqual(nextValue, currentValue);
        }
    }
}