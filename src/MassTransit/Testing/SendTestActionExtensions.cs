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
namespace MassTransit.Testing
{
	using System;
	using ActionConfigurators;
	using TestInstanceConfigurators;

	public static class SendTestActionExtensions
	{
		public static void Send<TMessage>(this TestInstanceConfigurator<BusTestScenario> configurator, TMessage message)
			where TMessage : class
		{
			var actionConfigurator = new SendTestActionConfigurator<BusTestScenario, TMessage>(x => x.Bus.Endpoint, message);

			configurator.AddActionConfigurator(actionConfigurator);
		}

		public static void Send<TMessage>(this TestInstanceConfigurator<BusTestScenario> configurator, TMessage message,
		                                  Action<ISendContext<TMessage>> callback)
			where TMessage : class
		{
			var actionConfigurator = new SendTestActionConfigurator<BusTestScenario, TMessage>(x => x.Bus.Endpoint, message,
				(scenario, context) => callback(context));

			configurator.AddActionConfigurator(actionConfigurator);
		}

		public static void Send<TMessage>(this TestInstanceConfigurator<BusTestScenario> configurator, TMessage message,
		                                  Action<BusTestScenario, ISendContext<TMessage>> callback)
			where TMessage : class
		{
			var actionConfigurator = new SendTestActionConfigurator<BusTestScenario, TMessage>(x => x.Bus.Endpoint, message,
				callback);

			configurator.AddActionConfigurator(actionConfigurator);
		}

		public static void Send<TMessage>(this TestInstanceConfigurator<LocalRemoteTestScenario> configurator, TMessage message)
			where TMessage : class
		{
			var actionConfigurator = new SendTestActionConfigurator<LocalRemoteTestScenario, TMessage>(x => x.RemoteBus.Endpoint, message);

			configurator.AddActionConfigurator(actionConfigurator);
		}

		public static void Send<TMessage>(this TestInstanceConfigurator<LocalRemoteTestScenario> configurator, TMessage message,
		                                  Action<ISendContext<TMessage>> callback)
			where TMessage : class
		{
			var actionConfigurator = new SendTestActionConfigurator<LocalRemoteTestScenario, TMessage>(x => x.RemoteBus.Endpoint, message,
				(scenario, context) => callback(context));

			configurator.AddActionConfigurator(actionConfigurator);
		}

		public static void Send<TMessage>(this TestInstanceConfigurator<LocalRemoteTestScenario> configurator, TMessage message,
										  Action<LocalRemoteTestScenario, ISendContext<TMessage>> callback)
			where TMessage : class
		{
			var actionConfigurator = new SendTestActionConfigurator<LocalRemoteTestScenario, TMessage>(x => x.RemoteBus.Endpoint, message,
				callback);

			configurator.AddActionConfigurator(actionConfigurator);
		}
	}
}