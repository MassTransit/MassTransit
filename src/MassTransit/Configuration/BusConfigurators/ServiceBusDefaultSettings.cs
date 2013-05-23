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
namespace MassTransit.BusConfigurators
{
	using System;
	using Magnum.Extensions;

	public class ServiceBusDefaultSettings
	{
		public ServiceBusDefaultSettings()
		{
			AutoStart = true;
			ReceiveTimeout = 3.Seconds();
		    ShutdownTimeout = 60.Seconds();
			ConcurrentReceiverLimit = 1;
			ConcurrentConsumerLimit = Environment.ProcessorCount*4;
			Network = Environment.MachineName.ToLowerInvariant();
		    EnablePerformanceCounters = true;
		}

		public bool AutoStart { get; set; }
		public int ConcurrentConsumerLimit { get; set; }
		public int ConcurrentReceiverLimit { get; set; }
		public IEndpointCache EndpointCache { get; set; }
		public TimeSpan ReceiveTimeout { get; set; }
        public TimeSpan ShutdownTimeout { get; set; }
		public string Network { get; set; }
		public bool EnablePerformanceCounters { get; set; }
	}
}