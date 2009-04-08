// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Configuration
{
	using System;
	using Internal;

	/// <summary>
	/// Enables the configuration of the service bus when it is being created
	/// </summary>
	public interface IServiceBusConfigurator :
		IServiceBusConfiguratorDefaults
	{
		/// <summary>
		/// Specify the endpoint from which messages should be read
		/// </summary>
		/// <param name="uriString">The uri of the endpoint</param>
		void ReceiveFrom(string uriString);

		/// <summary>
		/// Specify the endpoint from which messages should be read
		/// </summary>
		/// <param name="uri">The uri of the endpoint</param>
		void ReceiveFrom(Uri uri);

		/// <summary>
		/// Configure a service for use by the service bus
		/// </summary>
		void ConfigureService<TServiceConfigurator>(Action<TServiceConfigurator> configure)
			where TServiceConfigurator : IServiceConfigurator, new();

		/// <summary>
		/// Adds a service for use by the service bus with the default configuration
		/// </summary>
		void AddService<TService>()
			where TService : IBusService;

		/// <summary>
		/// Sets the endpoint to be purged before starting the service bus
		/// </summary>
		void PurgeBeforeStarting();

		/// <summary>
		/// Specifies a control bus for the service bus
		/// </summary>
		/// <param name="bus">The bus instance to use as the control bus</param>
		void UseControlBus(IControlBus bus);
	}
}