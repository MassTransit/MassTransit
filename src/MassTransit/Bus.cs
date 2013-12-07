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
	using Exceptions;

	/// <summary>
	/// This is a static singleton instance of an IServiceBus. While it goes
	/// against my very soul, it is here to ensure consistent usage of MassTransit
	/// as a singleton. It is highly recommended that <see cref="ServiceBusFactory.New"/> be
	/// used instead and the application maintain the reference to the IServiceBus.
	/// </summary>
	public static class Bus
	{
		static IServiceBus _instance;

		/// <summary>
		/// The configured instance of the service bus.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// If you call this method before you call
		/// <see cref="Initialize"/>.
		/// </exception>
		public static IServiceBus Instance
		{
			get
			{
				if (_instance == null)
					throw new InvalidOperationException("You must call Bus.Initialize before accessing Bus.Instance.");

				return _instance;
			}
		}

		/// <summary>
		/// Call to initialize the service bus instance, including any configuration.
		/// </summary>
		/// <param name="configure">A lambda/action that does the bus configugration.</param>
		/// <exception cref="ConfigurationException">
		/// If the bus has already been initialized by a call
		/// to this method.</exception>
		public static void Initialize(Action<ServiceBusConfigurator> configure)
		{
			if (_instance != null)
				throw new ConfigurationException("Bus.Instance has already been initialized. Call Shutdown first.");

			_instance = ServiceBusFactory.New(configure);
		}

		/// <summary>
		/// Shuts down the service bus and disposes any used resources
		/// </summary>
		public static void Shutdown()
		{
			if (_instance == null)
				return;

			_instance.Dispose();
			_instance = null;
		}
	}
}