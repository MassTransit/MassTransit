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
namespace MassTransit.Services.HealthMonitoring.Configuration
{
	using System;
	using Internal;
	using MassTransit.Configuration;

	public class HealthClientConfigurator :
		IServiceConfigurator
	{
		private int _intervalInSeconds;

		public Type ServiceType
		{
			get { return typeof (HealthClient); }
		}

		public IBusService Create(IServiceBus bus, IObjectBuilder builder)
		{
			var service = new HealthClient(_intervalInSeconds);

			return service;
		}

		public void SetHeartbeatInterval(int seconds)
		{
			_intervalInSeconds = seconds;
		}
	}
}