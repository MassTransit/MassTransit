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
namespace MassTransit.Transports
{
	using System;
	using System.Transactions;

	public interface ITransportSettings
	{
		/// <summary>
		/// The address of the endpoint/transport
		/// </summary>
		IEndpointAddress Address { get; }

		/// <summary>
		/// The transport should be created if it was not found
		/// </summary>
		bool CreateIfMissing { get; }

		/// <summary>
		/// The isolation level to use with the transaction if a transactional transport is used
		/// </summary>
		IsolationLevel IsolationLevel { get; }

		/// <summary>
		/// If the transport should purge any existing messages before reading from the queue
		/// </summary>
		bool PurgeExistingMessages { get; }

		/// <summary>
		/// if the transactional queue is requested and required it will throw an exception if the queue 
		/// exists and is not transactional
		/// </summary>
		bool RequireTransactional { get; }

		/// <summary>
		/// The timeout for the transaction if System.Transactions is supported
		/// </summary>
		TimeSpan TransactionTimeout { get; }

		/// <summary>
		/// True if the endpoint should be transactional. If Transactional is true and the endpoint already
		/// exists and is not transactional, an exception will be thrown.
		/// </summary>
		bool Transactional { get; }
	}
}