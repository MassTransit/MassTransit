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
namespace MassTransit
{
	using System;
	using BusConfigurators;

	public static class ControlBusConfigurationExtensions
	{
		/// <summary>
		/// Create a control bus using the default settings and associate it with the ServiceBus being configured.
		/// </summary>
		/// <param name="configurator"></param>
		public static void UseControlBus(this ServiceBusConfigurator configurator)
		{
			UseControlBus(configurator, x => { });
		}

		/// <summary>
		/// Create a control bus, associate it with the ServiceBus being configured, and allow for customization using
		/// the specified method.
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="configure"></param>
		public static void UseControlBus(this ServiceBusConfigurator configurator, Action<ControlBusConfigurator> configure)
		{
			var controlBusConfigurator = new ControlBusConfiguratorImpl();

			configure(controlBusConfigurator);

			configurator.AddBusConfigurator(controlBusConfigurator);
		}
	}
}