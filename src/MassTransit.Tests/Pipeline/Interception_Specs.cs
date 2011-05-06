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
namespace MassTransit.Tests.Pipeline
{
	using Magnum.TestFramework;
	using TextFixtures;

	[Scenario]
	public class When_intercepting_messages_as_they_are_published
		: LoopbackTestFixture
	{
		[When]
		public void Intercepting_messages_as_they_are_published()
		{

		}

		protected override void ConfigureLocalBus(BusConfigurators.ServiceBusConfigurator configurator)
		{
			base.ConfigureLocalBus(configurator);
		}

		[Then]
		public void Should_receive_the_message_before_the_pipeline()
		{
		}
	}
}