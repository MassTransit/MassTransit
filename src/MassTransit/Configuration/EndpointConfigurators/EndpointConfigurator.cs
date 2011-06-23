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
namespace MassTransit.EndpointConfigurators
{
	using System;
	using System.Transactions;
	using Serialization;
	using Transports;

	/// <summary>
	/// Configure the endpoint
	/// </summary>
	public interface EndpointConfigurator
	{
		/// <summary>
		/// Specify a serializer for this endpoint (overrides the default)
		/// </summary>
		EndpointConfigurator UseSerializer(IMessageSerializer serializer);

		/// <summary>
		/// Overrides the default error address with a new error address
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		EndpointConfigurator SetErrorAddress(Uri uri);

		/// <summary>
		/// Overrides the transport factory when the error transport is created, to modify the behavior
		/// </summary>
		/// <param name="transportFactory"></param>
		EndpointConfigurator SetTransportFactory(
			Func<ITransportFactory, ITransportSettings, IDuplexTransport> transportFactory);

		/// <summary>
		/// Overrides the transport factory when the error transport is created, to modify the behavior
		/// </summary>
		/// <param name="errorTransportFactory"></param>
		EndpointConfigurator SetErrorTransportFactory(
			Func<ITransportFactory, ITransportSettings, IOutboundTransport> errorTransportFactory);

		/// <summary>
		/// Remove any existing messages from the endpoint when it is created
		/// </summary>
		/// <returns></returns>
		EndpointConfigurator PurgeExistingMessages();

		/// <summary>
		/// Creates the endpoint as transactional if it needs to be created
		/// </summary>
		/// <returns></returns>
		EndpointConfigurator CreateTransactional();

		/// <summary>
		/// Creates the endpoint if it is missing
		/// </summary>
		/// <returns></returns>
		EndpointConfigurator CreateIfMissing();

		/// <summary>
		/// Sets the transaction timeout for the endpoint
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		EndpointConfigurator SetTransactionTimeout(TimeSpan timeout);

		/// <summary>
		/// Sets the isolation level for the endpoint if it is transactional
		/// </summary>
		/// <param name="isolationLevel"></param>
		/// <returns></returns>
		EndpointConfigurator SetIsolationLevel(IsolationLevel isolationLevel);
	}
}