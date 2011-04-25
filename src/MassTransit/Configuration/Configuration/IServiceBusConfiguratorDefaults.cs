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

	/// <summary>
	/// Enables the configuration of items that can be shared between multiple ServiceBus instances
	/// </summary>
	[Obsolete]
	public interface IServiceBusConfiguratorDefaults
	{
		/// <summary>
		/// Specify the IObjectBuilder to use when creating objects
		/// </summary>
		/// <param name="objectBuilder"></param>
		void SetObjectBuilder(IObjectBuilder objectBuilder);

	    /// <summary>
	    /// Specify the IEndpointFactory to use
	    /// </summary>
	    /// <param name="endpointCache"></param>
        void SetEndpointFactory(IEndpointCache endpointCache);

		/// <summary>
		/// Do not start the ServiceBus when it is created
		/// </summary>
		void DisableAutoStart();

		/// <summary>
		/// Set the default receive timeout for newly created buses
		/// This is nice to set lower for quicker running tests
		/// </summary>
		/// <param name="receiveTimeout"></param>
		void SetReceiveTimeout(TimeSpan receiveTimeout);

		/// <summary>
		/// Set the maximum number of concurrent consumers that can be active at any time. For consumers
		/// performing high-latency, low-CPU operations, settings this number higher may increase throughput.
		/// </summary>
		/// <param name="concurrentConsumerLimit"></param>
		void SetConcurrentConsumerLimit(int concurrentConsumerLimit);

		/// <summary>
		/// Set the maximum number of concurrent threads that are receiving messages from the endpoint.
		/// In most cases, this can be left at the default value of 1, but can be increased when using
		/// transactional queues.
		/// </summary>
		/// <param name="concurrentReceiverLimit"></param>
		void SetConcurrentReceiverLimit(int concurrentReceiverLimit);
	}
}