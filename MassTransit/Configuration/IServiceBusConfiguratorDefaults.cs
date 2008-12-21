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

	public interface IServiceBusConfiguratorDefaults
	{
		/// <summary>
		/// Set the default object builder for all service buses created
		/// </summary>
		/// <param name="objectBuilder"></param>
		void SetObjectBuilder(IObjectBuilder objectBuilder);

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
		/// <param name="threadLimit"></param>
		void SetThreadLimit(int threadLimit);

		/// <summary>
		/// Set the maximum number of concurrent threads that are receiving messages from the endpoint.
		/// In most cases, this can be left at the default value of 1, but can be increased when using
		/// transactional queues.
		/// </summary>
		/// <param name="receiveThreadLimit"></param>
		void SetReceiveThreadLimit(int receiveThreadLimit);
	}
}