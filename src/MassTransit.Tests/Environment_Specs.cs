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
namespace MassTransit.Tests
{
	using Advanced;
	using BusConfigurators;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using MassTransit.Configuration;

	[Scenario]
	public class When_specifying_environments_for_a_bus
	{
		[Then]
		public void Should_allow_inline_environment()
		{
			using (IServiceBus bus = ServiceBusFactory.New(x =>
				{
					x.ReceiveFrom("loopback://localhost/my_queue");
					x.SetReceiveTimeout(10.Milliseconds());

					x.Environments(e =>
						{
							e.Add("development", y => { y.SetConcurrentConsumerLimit(1); });

							e.Select("development");
						});
				}))
			{
				((ServiceBus) bus).MaximumConsumerThreads.ShouldEqual(1);
			}
		}

		[Then]
		public void Should_allow_class_level_environments()
		{
			using (IServiceBus bus = ServiceBusFactory.New(x =>
				{
					x.ReceiveFrom("loopback://localhost/my_queue");
					x.SetReceiveTimeout(10.Milliseconds());

					x.Environments(e =>
						{
							e.Add<Production>();

							e.Select("production");
						});
				}))
			{
				((ServiceBus) bus).MaximumConsumerThreads.ShouldEqual(7);
			}
		}

		[Then]
		public void Should_allow_selection_by_app_setting()
		{
			using (IServiceBus bus = ServiceBusFactory.New(x =>
				{
					x.ReceiveFrom("loopback://localhost/my_queue");
					x.SetReceiveTimeout(10.Milliseconds());

					x.Environments(e =>
						{
							e.Add<Production>();
							e.Add<Development>();

							e.SelectByAppSetting("environmentName");
						});
				}))
			{
				((ServiceBus) bus).MaximumConsumerThreads.ShouldEqual(1);
			}
		}

		class Production :
			IServiceBusEnvironment
		{
			public void Configure(ServiceBusConfigurator configurator)
			{
				configurator.SetConcurrentConsumerLimit(7);
			}
		}

		class Development :
			IServiceBusEnvironment
		{
			public void Configure(ServiceBusConfigurator configurator)
			{
				configurator.SetConcurrentConsumerLimit(1);
			}
		}
	}
}