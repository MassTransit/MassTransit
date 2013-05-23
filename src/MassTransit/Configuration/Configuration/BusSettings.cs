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
namespace MassTransit.Configuration
{
	using System;

	/// <summary>
	/// Interface with bus settings. Each bus needs to have its own settings,
	/// and you may have multiple buses with different sets of settings, active,
	/// at any given point in time of your application.
	/// </summary>
	public interface BusSettings
	{
		Uri InputAddress { get; }

		Action BeforeConsume { get; }
		Action AfterConsume { get; }

		bool AutoStart { get; }

		IEndpointCache EndpointCache { get; }

		TimeSpan ReceiveTimeout { get; }

        TimeSpan ShutdownTimeout { get; }

		int ConcurrentReceiverLimit { get; }

		int ConcurrentConsumerLimit { get; }

		string Network { get; }

	    bool EnablePerformanceCounters { get; }
	}
}