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
namespace MassTransit.Transports.Msmq
{
	using System.Collections.Generic;

	/// <summary>
	/// Provides a management interface to an endpoint
	/// Since this has more than one method, these may get broken up
	/// into individual interfaces such as ICreateEndpoint, ICountEndpoint, etc.
	/// </summary>
	public interface IEndpointManagement
	{
		/// <summary>
		/// Creates the endpoint if it does not exist using the specified transaction mode
		/// </summary>
		/// <param name="transactional"></param>
		void Create(bool transactional);

		/// <summary>
		/// Returns the number of messages waiting to be received on the endpoint
		/// </summary>
		/// <returns></returns>
		long Count();

		/// <summary>
		/// Returns the number of messages waiting to be received or the limit if the limit is received
		/// </summary>
		/// <param name="limit"></param>
		/// <returns></returns>
		long Count(long limit);

		/// <summary>
		/// Purge all messages from the endpoint
		/// </summary>
		void Purge();

		/// <summary>
		/// Returns true if the endpoint exists
		/// </summary>
		bool Exists { get; }

        /// <summary>
        /// Returns a dictionary containing the message types and the number of messages for each type. 
        /// </summary>
        Dictionary<string, int> MessageTypes();

		/// <summary>
		/// Returns true if the endpoint supports transactions
		/// </summary>
		bool IsTransactional { get; }
	}
}