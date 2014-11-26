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
	using Magnum.TestFramework;
	using MassTransit.Testing;

	[Scenario]
	public class When_a_consumer_with_multiple_message_consumers_is_tested
	{
		ConsumerTest<BusTestScenario, Testsumer> _test;

		[When]
		public void A_consumer_is_being_tested()
		{
			_test = TestFactory.ForConsumer<Testsumer>()
				.New(x =>
					{
						x.ConstructUsing(() => new Testsumer());

					//	x.Send(new A(), (scenario, context) => context.SendResponseTo(scenario.Bus));
						//x.Send(new B(), (scenario, context) => context.SendResponseTo(scenario.Bus));
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
		public void Should_have_sent_the_aa_response_from_the_consumer()
		{
			_test.Sent.Any<Aa>().ShouldBeTrue();
		}

		[Then]
		public void Should_have_sent_the_bb_response_from_the_consumer()
		{
			_test.Sent.Any<Bb>().ShouldBeTrue();
		}

		[Then]
		public void Should_have_called_the_consumer_a_method()
		{
			_test.Consumer.Received.Any<A>().ShouldBeTrue();
		}

		[Then]
		public void Should_have_called_the_consumer_b_method()
		{
			_test.Consumer.Received.Any<B>().ShouldBeTrue();
		}

		class Testsumer :
			Consumes<A>.All,
			Consumes<B>.All
		{
			public void Consume(A message)
			{
				this.MessageContext<A>().Respond(new Aa());
			}

			public void Consume(B message)
			{
				this.MessageContext<B>().Respond(new Bb());
			}
		}

		class A
		{
		}

		class Aa
		{
		}

		class B
		{
		}

		class Bb
		{
		}
	}
}