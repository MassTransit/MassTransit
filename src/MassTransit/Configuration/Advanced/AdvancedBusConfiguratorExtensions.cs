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
namespace MassTransit.Advanced
{
	using System;
	using BusConfigurators;

	public static class AdvancedBusConfiguratorExtensions
	{
		/// <summary>
		/// Sets the number of concurrent receive threads that can execute simultaneously. In many cases, such as when
		/// using non-transactional transports, this can lead to very-bad-things(TM)
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="concurrentReceiverLimit"></param>
		public static void SetConcurrentReceiverLimit(this ServiceBusConfigurator configurator, int concurrentReceiverLimit)
		{
			var controlBusConfigurator =
				new PostCreateBusBuilderConfiguratorImpl(bus => { bus.ConcurrentReceiveThreads = concurrentReceiverLimit; });

			configurator.AddBusConfigurator(controlBusConfigurator);
		}

		/// <summary>
		/// Sets the receive timeout on the service bus endpoint receiver. Settings this to a lower value has limited
		/// benefit other than in unit testing to reduce bus disposal time
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="receiveTimeout"></param>
		public static void SetReceiveTimeout(this ServiceBusConfigurator configurator, TimeSpan receiveTimeout)
		{
			var controlBusConfigurator = new PostCreateBusBuilderConfiguratorImpl(bus => { bus.ReceiveTimeout = receiveTimeout; });

			configurator.AddBusConfigurator(controlBusConfigurator);
		}
	}
}