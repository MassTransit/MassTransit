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
namespace MassTransit.Tests.Saga.Locator
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using Messages;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class SagaExpression_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Matching_by_property_should_be_happy()
        {
            Expression<Func<SimpleSaga, ObservableSagaMessage, bool>> selector = (s, m) => s.Name == m.Name;

            Expression<Func<SimpleSaga, bool>> filter =
                new SagaFilterExpressionConverter<SimpleSaga, ObservableSagaMessage>(_observeSaga).Convert(selector);
            Trace.WriteLine(filter.ToString());

            Guid? matches = await _repository.ShouldContainSaga(filter, TestTimeout);

            Assert.IsTrue(matches.HasValue);
        }

        [Test]
        public async Task The_saga_expression_should_be_converted_down_to_a_saga_only_filter()
        {
            Expression<Func<SimpleSaga, InitiateSimpleSaga, bool>> selector =
                (s, m) => s.CorrelationId == m.CorrelationId;

            Expression<Func<SimpleSaga, bool>> filter =
                new SagaFilterExpressionConverter<SimpleSaga, InitiateSimpleSaga>(_initiateSaga).Convert(selector);
            Trace.WriteLine(filter.ToString());

            Guid? matches = await _repository.ShouldContainSaga(filter, TestTimeout);

            Assert.IsTrue(matches.HasValue);
        }

        public SagaExpression_Specs()
        {
            _repository = new InMemorySagaRepository<SimpleSaga>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_repository);
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _sagaId = NewId.NextGuid();
            _initiateSaga = new InitiateSimpleSaga {CorrelationId = _sagaId, Name = "Chris"};

            InputQueueSendEndpoint.Send(_initiateSaga)
                .Wait(TestCancellationToken);

            _repository.ShouldContainSaga(_sagaId, TestTimeout)
                .Wait(TestCancellationToken);

            _otherSagaId = Guid.NewGuid();
            _initiateOtherSaga = new InitiateSimpleSaga {CorrelationId = _otherSagaId, Name = "Dru"};

            InputQueueSendEndpoint.Send(_initiateOtherSaga)
                .Wait(TestCancellationToken);

            _repository.ShouldContainSaga(_otherSagaId, TestTimeout)
                .Wait(TestCancellationToken);

            _observeSaga = new ObservableSagaMessage {Name = "Chris"};
        }

        Guid _sagaId;
        InitiateSimpleSaga _initiateSaga;
        readonly InMemorySagaRepository<SimpleSaga> _repository;
        Guid _otherSagaId;
        ObservableSagaMessage _observeSaga;
        InitiateSimpleSaga _initiateOtherSaga;
    }
}