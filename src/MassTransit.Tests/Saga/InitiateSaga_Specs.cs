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
namespace MassTransit.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Introspection;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using Messages;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class When_an_initiating_message_for_a_saga_arrives :
        InMemoryTestFixture
    {
        [Test, Explicit]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        [Test]
        public async Task The_saga_should_be_created_when_an_initiating_message_is_received()
        {
            var message = new InitiateSimpleSaga(_sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? sagaId = await _repository.ShouldContainSaga(_sagaId, TestTimeout);

            sagaId.HasValue.ShouldBe(true);
        }

        public When_an_initiating_message_for_a_saga_arrives()
        {
            _repository = new InMemorySagaRepository<SimpleSaga>();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _sagaId = Guid.NewGuid();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.UseRetry(x => x.None());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseRetry(x => x.Immediate(2));
            configurator.Saga(_repository);
        }

        Guid _sagaId;
        InMemorySagaRepository<SimpleSaga> _repository;
    }


    [TestFixture]
    public class When_an_initiating_message_for_an_existing_saga_arrives :
        InMemoryTestFixture
    {
        [Test]
        public async Task The_message_should_fault()
        {
            Task<ConsumeContext<Fault<InitiateSimpleSaga>>> faulted = ConnectPublishHandler<Fault<InitiateSimpleSaga>>();

            var message = new InitiateSimpleSaga(_sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? sagaId = await _repository.ShouldContainSaga(_sagaId, TestTimeout);

            sagaId.HasValue.ShouldBe(true);

            await InputQueueSendEndpoint.Send(message);

            await faulted;
        }

        public When_an_initiating_message_for_an_existing_saga_arrives()
        {
            _repository = new InMemorySagaRepository<SimpleSaga>();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _sagaId = Guid.NewGuid();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_repository);
        }

        Guid _sagaId;
        InMemorySagaRepository<SimpleSaga> _repository;
    }


    [TestFixture]
    public class When_an_initiating_and_orchestrated_message_for_a_saga_arrives :
        InMemoryTestFixture
    {
        [Test]
        public async Task The_saga_should_be_loaded()
        {
            await InputQueueSendEndpoint.Send(new InitiateSimpleSaga(_sagaId));

            Guid? sagaId = await _repository.ShouldContainSaga(x => x.Initiated, TestTimeout);

            await InputQueueSendEndpoint.Send(new CompleteSimpleSaga(_sagaId));

            sagaId = await _repository.ShouldContainSaga(x => x.Completed, TestTimeout);

            sagaId.HasValue.ShouldBe(true);
        }

        public When_an_initiating_and_orchestrated_message_for_a_saga_arrives()
        {
            _repository = new InMemorySagaRepository<SimpleSaga>();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _sagaId = Guid.NewGuid();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_repository);
        }

        Guid _sagaId;
        InMemorySagaRepository<SimpleSaga> _repository;
    }


    [TestFixture]
    public class When_an_initiating_and_observed_message_for_a_saga_arrives :
        InMemoryTestFixture
    {
        [Test]
        public async Task The_saga_should_be_loaded()
        {
            await InputQueueSendEndpoint.Send(new InitiateSimpleSaga(_sagaId) {Name = "Chris"});

            Guid? sagaId = await _repository.ShouldContainSaga(x => x.Initiated, TestTimeout);

            await InputQueueSendEndpoint.Send(new ObservableSagaMessage {Name = "Chris"});

            sagaId = await _repository.ShouldContainSaga(x => x.Observed, TestTimeout);

            sagaId.HasValue.ShouldBe(true);
        }

        public When_an_initiating_and_observed_message_for_a_saga_arrives()
        {
            _repository = new InMemorySagaRepository<SimpleSaga>();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _sagaId = Guid.NewGuid();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_repository);
        }

        Guid _sagaId;
        InMemorySagaRepository<SimpleSaga> _repository;
    }


    [TestFixture]
    public class When_an_existing_saga_receives_an_initiating_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task An_exception_should_be_thrown()
        {
            var message = new InitiateSimpleSaga(_sagaId);

            await BusSendEndpoint.Send(message);


            try
            {
                await BusSendEndpoint.Send(message);
            }
            catch (SagaException sex)
            {
                Assert.AreEqual(sex.MessageType, typeof(InitiateSimpleSaga));
            }
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            _sagaId = Guid.NewGuid();

            _repository = new InMemorySagaRepository<SimpleSaga>();

            Bus.ConnectSaga(_repository);
        }

        Guid _sagaId;
        InMemorySagaRepository<SimpleSaga> _repository;
    }
}