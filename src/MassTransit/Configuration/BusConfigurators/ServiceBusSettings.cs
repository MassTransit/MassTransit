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
	using Configuration;

	public class ServiceBusSettings :
		BusSettings
	{
		public ServiceBusSettings(ServiceBusDefaultSettings defaultSettings)
		{
			AutoStart = defaultSettings.AutoStart;
			ConcurrentConsumerLimit = defaultSettings.ConcurrentConsumerLimit;
			ConcurrentReceiverLimit = defaultSettings.ConcurrentReceiverLimit;
			ReceiveTimeout = defaultSettings.ReceiveTimeout;
		    ShutdownTimeout = defaultSettings.ShutdownTimeout;
			EndpointCache = defaultSettings.EndpointCache;
			Network = defaultSettings.Network;
		    EnablePerformanceCounters = defaultSettings.EnablePerformanceCounters;
		}

		public ServiceBusSettings(BusSettings settings)
		{
			AutoStart = settings.AutoStart;
			ConcurrentConsumerLimit = settings.ConcurrentConsumerLimit;
			ConcurrentReceiverLimit = settings.ConcurrentReceiverLimit;
			ReceiveTimeout = settings.ReceiveTimeout;
		    ShutdownTimeout = settings.ShutdownTimeout;
			EndpointCache = settings.EndpointCache;
			Network = settings.Network;
		    EnablePerformanceCounters = settings.EnablePerformanceCounters;
		}

		public ServiceBusSettings()
		{
		}

		public IEndpointCache EndpointCache { get; set; }
		public TimeSpan ReceiveTimeout { get; set; }
	    public TimeSpan ShutdownTimeout { get; set; }
	    public int ConcurrentReceiverLimit { get; set; }
		public int ConcurrentConsumerLimit { get; set; }

		public string Network { get; set; }
	    public bool EnablePerformanceCounters { get; set; }

	    public Action BeforeConsume { get; set; }
		public Action AfterConsume { get; set; }

		public bool AutoStart { get; set; }
		public Uri InputAddress { get; set; }
	}
}