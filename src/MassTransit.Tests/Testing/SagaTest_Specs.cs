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
	using System.Linq.Expressions;
	using Magnum.TestFramework;
	using MassTransit.Saga;
	using MassTransit.Testing;

	[Scenario]
	public class When_a_saga_is_being_tested
	{
		SagaTest<BusTestScenario, TestSaga> _test;
		Guid _sagaId;

		[When]
		public void A_consumer_is_being_tested()
		{
			_sagaId = Guid.NewGuid();

			_test = TestFactory.ForSaga<TestSaga>()
				.InSingleBusScenario()
				.New(x =>
					{
						x.Send(new A{CorrelationId = _sagaId});
					});

			_test.Execute();
		}

		[Finally]
		public void Teardown()
		{
			_test.Dispose();
			_test = null;
		}


		[Then]
		public void Should_send_the_initial_message_to_the_consumer()
		{
			_test.Sent.Any<A>().ShouldBeTrue();
		}

		[Then]
		public void Should_receive_the_message_type_a()
		{
			_test.Received.Any<A>().ShouldBeTrue();
		}

		[Then]
		public void Should_create_a_new_saga_for_the_message()
		{
			_test.Saga.Created.Any(x => x.CorrelationId == _sagaId).ShouldBeTrue();
		}

		[Then]
		public void Should_have_called_the_consumer_method()
		{
			_test.Saga.Received.Any<A>().ShouldBeTrue();
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

			public void Consume(A message)
			{
			}

			public void Consume(B message)
			{
			}

			public void Consume(C message)
			{
			}

			public Expression<Func<TestSaga, C, bool>> GetBindExpression()
			{
				return (saga, message) => saga.CorrelationId.ToString() == message.CorrelationId;
			}

			public Guid CorrelationId { get; private set; }
			public IServiceBus Bus { get; set; }
		}

		class A :
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