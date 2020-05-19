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
namespace MassTransit.Tests.Testing
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using NUnit.Framework;
	using MassTransit.Saga;
	using MassTransit.Testing;
	using Shouldly;

    public class When_a_saga_is_being_tested
	{
		Guid _sagaId;
		string _testValueA;
        InMemoryTestHarness _harness;
        SagaTestHarness<TestSaga> _saga;

        [OneTimeSetUp]
		public async Task A_saga_is_being_tested()
		{
			_sagaId = Guid.NewGuid();
			_testValueA = "TestValueA";

            _harness = new InMemoryTestHarness();
            _saga = _harness.Saga<TestSaga>();

            await _harness.Start();

            await _harness.InputQueueSendEndpoint.Send(new A
							{
								CorrelationId = _sagaId, 
								Value = _testValueA
							});

		}

		[OneTimeTearDown]
		public async Task Teardown()
		{
			await _harness.Stop();
		}


		[Test]
		public void Should_send_the_initial_message_to_the_consumer()
		{
            _harness.Sent.Select<A>().Any().ShouldBe(true);
		}

		[Test]
		public void Should_receive_the_message_type_a()
		{
            _harness.Consumed.Select<A>().Any().ShouldBe(true);
		}

		[Test]
		public void Should_create_a_new_saga_for_the_message()
		{
            _saga.Created.Select(x => x.CorrelationId == _sagaId).Any().ShouldBe(true);
		}

		[Test]
		public void Should_have_the_saga_instance_with_the_value()
		{
			TestSaga saga = _saga.Created.Contains(_sagaId);
			saga.ShouldNotBe(null);

			saga.ValueA.ShouldBe(_testValueA);
		}

		[Test]
		public void Should_have_published_event_message()
		{
			_harness.Published.Select<Aa>().Any().ShouldBe(true);
		}

		[Test]
		public void Should_have_called_the_consumer_method()
		{
            _saga.Consumed.Select<A>().Any().ShouldBe(true);
		}

		class TestSaga :
			ISaga,
			InitiatedBy<A>,
			Orchestrates<B>,
			Observes<C, TestSaga>
		{
			protected TestSaga()
			{
			}

			public TestSaga(Guid correlationId)
			{
				CorrelationId = correlationId;
			}

			public string ValueA { get; private set; }

            public async Task Consume(ConsumeContext<A> context)
			{
				ValueA = context.Message.Value;
				await context.Publish(new Aa {CorrelationId = CorrelationId});
			}

			public Guid CorrelationId { get; set; }

            public async Task Consume(ConsumeContext<C> message)
			{
			}

		    public Expression<Func<TestSaga, C, bool>> CorrelationExpression
		    {
		        get { return (saga, message) => saga.CorrelationId.ToString() == message.CorrelationId; }
		    }

		    public async Task Consume(ConsumeContext<B> message)
			{
			}
		}

		class A :
			CorrelatedBy<Guid>
		{
			public string Value { get; set; }
			public Guid CorrelationId { get; set; }
		}

		class Aa :
			CorrelatedBy<Guid>
		{
			public Guid CorrelationId { get; set; }
		}

		class B :
			CorrelatedBy<Guid>
		{
			public Guid CorrelationId { get; set; }
		}

		class C
		{
			public string CorrelationId { get; set; }
		}
	}
}