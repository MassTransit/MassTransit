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
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
	using MassTransit.Testing;
    using Shouldly;

    [Explicit]
    public class When_a_consumer_is_being_tested
	{
		IConsumerTest<IBusTestScenario, Testsumer> _test;

		[SetUp]
		public void A_consumer_is_being_tested()
		{
			_test = TestFactory.ForConsumer<Testsumer>()
				.New(x =>
					{
						x.UseConsumerFactory(() => new Testsumer());

						x.Send(new A());
					});

			_test.Execute();
		}

		[TearDown]
		public void Teardown()
		{
			_test.Dispose();
			_test = null;
		}


		[Test]
		public void Should_send_the_initial_message_to_the_consumer()
		{
			_test.Sent.Select<A>().Any().ShouldBe(true);
		}

		[Test]
		public void Should_have_sent_the_response_from_the_consumer()
		{
            _test.Sent.Select<B>().Any().ShouldBe(true);
		}

		[Test]
		public void Should_receive_the_message_type_a()
		{
            _test.Received.Select<A>().Any().ShouldBe(true);
		}

		[Test]
		public void Should_have_called_the_consumer_method()
		{
            _test.Consumer.Received.Select<A>().Any().ShouldBe(true);
		}

		class Testsumer :
			IConsumer<A>
		{
		    public async Task Consume(ConsumeContext<A> context)
		    {
		        await context.RespondAsync(new B());
		    }
		}

		class A
		{
		}

		class B
		{
		}
	}

	[Explicit]
	public class When_a_context_consumer_is_being_tested
	{
		IConsumerTest<IBusTestScenario, Testsumer> _test;

		[SetUp]
		public void A_consumer_is_being_tested()
		{
			_test = TestFactory.ForConsumer<Testsumer>()
				.New(x =>
					{
						x.UseConsumerFactory(() => new Testsumer());

						x.Send(new A(), (scenario, context) => context.ResponseAddress = scenario.Bus.Address);
					});

			_test.ExecuteAsync();
		}

		[TearDown]
		public void Teardown()
		{
			_test.Dispose();
			_test = null;
		}


		[Test]
		public void Should_send_the_initial_message_to_the_consumer()
		{
            _test.Sent.Select<A>().Any().ShouldBe(true);
		}

		[Test]
		public void Should_have_sent_the_response_from_the_consumer()
		{
            _test.Sent.Select<B>().Any().ShouldBe(true);
		}

		[Test]
		public void Should_receive_the_message_type_a()
		{
            _test.Received.Select<A>().Any().ShouldBe(true);
		}

		[Test]
		public void Should_have_called_the_consumer_method()
		{
            _test.Consumer.Received.Select<A>().Any().ShouldBe(true);
		}

		class Testsumer :
			IConsumer<A>
		{
			public Task Consume(ConsumeContext<A> context)
			{
				return context.RespondAsync(new B());
			}
		}

		class A
		{
		}

		class B
		{
		}
	}
}