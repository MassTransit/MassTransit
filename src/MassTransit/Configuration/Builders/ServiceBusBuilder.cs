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
namespace MassTransit.Builders
{
	using System;
	using BusServiceConfigurators;

	/// <summary>
	/// A ServiceBusBuilder includes everything for configuring a complete service bus instance,
	/// and is an extension of the BusBuilder (which can only build a limited, dependent bus)
	/// </summary>
	public interface ServiceBusBuilder :
		BusBuilder
	{
		/// <summary>
		/// Specifies a control bus to associate with the service bus once created
		/// </summary>
		/// <param name="controlBus"></param>
		void UseControlBus(IControlBus controlBus);

		/// <summary>
		/// Adds an action to be performed after bus creation to adjust settings, etc.
		/// but before the bus is started.
		/// </summary>
		/// <param name="postCreateAction"></param>
		void AddPostCreateAction(Action<ServiceBus> postCreateAction);

		/// <summary>
		/// Adds a bus service that will be started and stopped with the service bus 
		/// </summary>
		/// <param name="configurator"></param>
		void AddBusServiceConfigurator(BusServiceConfigurator configurator);
	}
}