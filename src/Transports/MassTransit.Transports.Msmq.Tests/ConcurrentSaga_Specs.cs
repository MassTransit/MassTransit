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
namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using BusConfigurators;
    using Logging;
    using Magnum.TestFramework;
    using NHibernateIntegration.Saga;
    using NHibernateIntegration.Tests.Sagas;
    using NUnit.Framework;
    using Saga;
    using TestFixtures;
    using TestFramework;

    [TestFixture, Category("Integration")]
    public class Sending_multiple_initiating_messages_should_not_fail_badly :
        MsmqConcurrentSagaTestFixtureBase
    {
        static readonly ILog _log =
            Logger.Get(typeof (Sending_multiple_initiating_messages_should_not_fail_badly));

        ISagaRepository<ConcurrentSaga> _sagaRepository;

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            _sagaRepository = new NHibernateSagaRepository<ConcurrentSaga>(SessionFactory);

            configurator.Subscribe(x => x.Saga(_sagaRepository));
        }

        [Test]
        public void Should_process_the_messages_in_order_and_not_at_the_same_time()
        {
            Guid transactionId = NewId.NextGuid();

            Trace.WriteLine("Creating transaction for " + transactionId);

            int startValue = 1;

            var startConcurrentSaga = new StartConcurrentSaga
                {CorrelationId = transactionId, Name = "Chris", Value = startValue};

            LocalBus.Publish(startConcurrentSaga);
            LocalBus.Publish(startConcurrentSaga);
            Trace.WriteLine("Just published the start message");

            _sagaRepository.ShouldContainSaga(transactionId).ShouldNotBeNull();

            int nextValue = 2;
            var continueConcurrentSaga = new ContinueConcurrentSaga {CorrelationId = transactionId, Value = nextValue};

            LocalBus.Publish(continueConcurrentSaga);
            Trace.WriteLine("Just published the continue message");

            _sagaRepository.ShouldContainSaga(x => x.CorrelationId == transactionId && x.Value == nextValue).
                ShouldNotBeNull();

            Thread.Sleep(8000);

            LocalEndpoint.ShouldNotContain<StartConcurrentSaga>();
            LocalErrorEndpoint.ShouldContain<StartConcurrentSaga>();
        }

        [Test]
        public void Should_process_the_saga_normally()
        {
            Guid transactionId = NewId.NextGuid();

            Trace.WriteLine("Creating transaction for " + transactionId);

            int startValue = 1;

            var startConcurrentSaga = new StartConcurrentSaga
                {CorrelationId = transactionId, Name = "Chris", Value = startValue};

            LocalBus.Publish(startConcurrentSaga);
            Trace.WriteLine("Just published the start message");

            _sagaRepository.ShouldContainSaga(transactionId).ShouldNotBeNull();

            int nextValue = 2;
            var continueConcurrentSaga = new ContinueConcurrentSaga {CorrelationId = transactionId, Value = nextValue};

            LocalBus.Publish(continueConcurrentSaga);
            Trace.WriteLine("Just published the continue message");

            ConcurrentSaga saga = _sagaRepository
                .ShouldContainSaga(x => x.CorrelationId == transactionId && x.Value == nextValue);
            saga.ShouldNotBeNull();
            saga.ShouldBeInState(ConcurrentSaga.Completed);
        }
    }
}