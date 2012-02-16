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
    using Magnum.Extensions;
    using MassTransit.Pipeline;
    using MassTransit.Saga;
    using NUnit.Framework;
    using TestFramework;

    [TestFixture]
    public class When_using_the_state_machine_with_noncorrelated_messages
    {
        [SetUp]
        public void Setup()
        {
            _sagaId = NewId.NextGuid();

            _repository = new InMemorySagaRepository<TestSaga>();

            var initiatePolicy = new InitiatingSagaPolicy<TestSaga, InitiateSimpleSaga>(x => x.CorrelationId, x => false);


            var message = new InitiateSimpleSaga(_sagaId);

            IConsumeContext<InitiateSimpleSaga> context = message.ToConsumeContext();
            _repository.GetSaga(context, message.CorrelationId,
                (i, c) => InstanceHandlerSelector.ForDataEvent(i, TestSaga.Initiate), initiatePolicy)
                .Each(x => x(context));

            message = new InitiateSimpleSaga(NewId.NextGuid());
            context = message.ToConsumeContext();
            _repository.GetSaga(context, message.CorrelationId,
                (i, c) => InstanceHandlerSelector.ForDataEvent(i, TestSaga.Initiate), initiatePolicy)
                .Each(x => x(context));
        }

        [TearDown]
        public void Teardown()
        {
            _repository = null;
        }

        Guid _sagaId;
        InMemorySagaRepository<TestSaga> _repository;

        [Test]
        public void A_nice_interface_should_be_available_for_defining_saga_locators()
        {
            _repository.ShouldContainSaga(_sagaId);
        }
    }

    public class NameMessage
    {
        public string Name { get; set; }
    }
}