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
	using NUnit.Framework;

	[TestFixture]
	public class Using_the_handler_test_factory
	{
		[SetUp]
		public void Setup()
		{
			_test = TestFactory.ForHandler<A>()
				.New(x => x.Send(new A()));

			_test.Execute();
		}

		[TearDown]
		public void Teardown()
		{
			_test.Dispose();
			_test = null;
		}

		HandlerTest<A> _test;

		class A
		{
		}

		[Test]
		public void Should_support_a_simple_handler()
		{
			_test.Handler.ReceivedMessages.Any().ShouldBeTrue();
		}
	}
}