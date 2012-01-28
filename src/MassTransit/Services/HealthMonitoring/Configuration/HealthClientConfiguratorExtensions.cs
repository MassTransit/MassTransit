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
	using BusConfigurators;
	using BusServiceConfigurators;
	using Services.HealthMonitoring.Configuration;

	public static class HealthClientConfiguratorExtensions
	{
		/// <summary>
		/// Specifies that the service bus will be using health monitoring. This means that
		/// the bus will publish heart beats and respond to ping messages. For more information,
		/// see http://readthedocs.org/docs/masstransit/en/latest/overview/standardservices.html
		/// </summary>
		/// <param name="configurator">Configurator that the extension method is invoked upon.</param>
		/// <param name="heartbeatInterval">The heartbeat interval in seconds (one heartbeat evey n seconds)</param>
		public static void UseHealthMonitoring(this ServiceBusConfigurator configurator, int heartbeatInterval)
		{
			var serviceConfigurator = new HealthClientConfigurator();
			serviceConfigurator.SetHeartbeatInterval(heartbeatInterval);

			var busConfigurator = new CustomBusServiceConfigurator(serviceConfigurator);

			configurator.AddBusConfigurator(busConfigurator);
		}
	}
}