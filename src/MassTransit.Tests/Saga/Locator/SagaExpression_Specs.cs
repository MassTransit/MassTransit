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
namespace MassTransit.Tests.Saga.Locator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using Magnum.Extensions;
    using MassTransit.Pipeline;
    using MassTransit.Saga;
    using MassTransit.Saga.Pipeline;
    using NUnit.Framework;

    [TestFixture]
    public class SagaExpression_Specs
    {
        [SetUp]
        public void Setup()
        {
            _repository = new InMemorySagaRepository<SimpleSaga>();
            var initiatePolicy = new InitiatingSagaPolicy<SimpleSaga, InitiateSimpleSaga>(x => x.CorrelationId,
                x => false);

            _sagaId = NewId.NextGuid();
            _initiateSaga = new InitiateSimpleSaga {CorrelationId = _sagaId, Name = "Chris"};
            //IConsumeContext<InitiateSimpleSaga> context = _initiateSaga.ToConsumeContext();
//            _repository.GetSaga(context, _sagaId,
//                (i, c) => InstanceHandlerSelector.ForInitiatedBy<SimpleSaga, InitiateSimpleSaga>(i), initiatePolicy)
//                .Each(x => x(context));

            _initiateOtherSaga = new InitiateSimpleSaga {CorrelationId = _otherSagaId, Name = "Dru"};

            _otherSagaId = Guid.NewGuid();
            //context = _initiateOtherSaga.ToConsumeContext();
//            _repository.GetSaga(context, _otherSagaId,
//                (i, c) => InstanceHandlerSelector.ForInitiatedBy<SimpleSaga, InitiateSimpleSaga>(i), initiatePolicy)
//                .Each(x => x(context));

            _observeSaga = new ObservableSagaMessage {Name = "Chris"};
        }

        Guid _sagaId;
        InitiateSimpleSaga _initiateSaga;
        InMemorySagaRepository<SimpleSaga> _repository;
        Guid _otherSagaId;
        ObservableSagaMessage _observeSaga;
        InitiateSimpleSaga _initiateOtherSaga;

        [Test]
        public void Matching_by_property_should_be_happy()
        {
            Expression<Func<SimpleSaga, ObservableSagaMessage, bool>> selector = (s, m) => s.Name == m.Name;

            Expression<Func<SimpleSaga, bool>> filter =
                new SagaFilterExpressionConverter<SimpleSaga, ObservableSagaMessage>(_observeSaga).Convert(selector);
            Trace.WriteLine(filter.ToString());

            IEnumerable<SimpleSaga> matches = _repository.Where(filter);

            Assert.AreEqual(1, matches.Count());
        }

        [Test]
        public void The_saga_expression_should_be_converted_down_to_a_saga_only_filter()
        {
            Expression<Func<SimpleSaga, InitiateSimpleSaga, bool>> selector =
                (s, m) => s.CorrelationId == m.CorrelationId;

            Expression<Func<SimpleSaga, bool>> filter =
                new SagaFilterExpressionConverter<SimpleSaga, InitiateSimpleSaga>(_initiateSaga).Convert(selector);
            Trace.WriteLine(filter.ToString());

            IEnumerable<SimpleSaga> matches = _repository.Where(filter);

            Assert.AreEqual(1, matches.Count());
        }
    }
}