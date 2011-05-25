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
	using MassTransit.Testing.Contexts;
	using MassTransit.Transports.Loopback;
	using NUnit.Framework;

	[TestFixture]
	public class When_using_a_context_builder_configurator
	{
		[Test]
		public void Should_allow_transport_factory_to_be_added()
		{
			ITestContext context =
				TestFactory.NewContext(x => { x.AddTransportFactory<LoopbackTransportFactory>(); });

			context.ShouldNotBeNull();
			context.ShouldBeAnInstanceOf<EndpointTestContext>();

			context.EndpointCache.ShouldNotBeNull();
			context.EndpointFactory.ShouldNotBeNull();
		}
	}
}