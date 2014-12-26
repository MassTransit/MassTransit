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
	using NUnit.Framework;
	using MassTransit.Testing;
	using Shouldly;

    [Explicit]
    public class When_a_handler_responds_to_a_message
	{
		HandlerTest<A> _test;

		[SetUp]
		public void A_handler_responds_to_a_message()
		{
			_test = TestFactory.ForHandler<A>()
				.New(x =>
					{
						x.Handler(async (context) => context.Respond(new B()));

						//x.Send(new A(), (scenario, context) => context.SendResponseTo(scenario.Bus));
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
		public void Should_have_sent_a_message_of_type_b()
		{
			_test.Sent.Any<B>().ShouldBe(true);
		}

		[Test]
		public void Should_support_a_simple_handler()
		{
			_test.Handler.Received.Any().ShouldBe(true);
		}

		class A
		{
		}

		class B
		{
		}
	}

	[Explicit]
	public class When_a_handler_responds_to_a_message_using_context
	{
		HandlerTest<A> _test;

		[SetUp]
		public void A_handler_responds_to_a_message_using_context()
		{
			_test = TestFactory.ForHandler<A>()
				.New(x =>
					{
						x.Handler(async (context) => context.Respond(new B()));

						//x.Send(new A(), (scenario, context) => context.SendResponseTo(scenario.Bus));
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
		public void Should_have_sent_a_message_of_type_b()
		{
			_test.Sent.Any<B>().ShouldBe(true);
		}

		[Test]
		public void Should_support_a_simple_handler()
		{
			_test.Handler.Received.Any().ShouldBe(true);
		}

		class A
		{
		}

		class B
		{
		}
	}
}