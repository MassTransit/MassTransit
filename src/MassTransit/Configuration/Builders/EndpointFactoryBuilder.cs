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
namespace MassTransit.Builders
{
	using System;
	using System.Transactions;
	using Serialization;
	using Transports;

	public interface EndpointFactoryBuilder
	{
		/// <summary>
		/// Create the endpoint factory
		/// </summary>
		/// <returns></returns>
		IEndpointFactory Build();

		/// <summary>
		/// Sets the default serializer used for endpoints
		/// </summary>
		void SetDefaultSerializer(IMessageSerializer serializerFactory);

		/// <summary>
		/// Sets the default transaction timeout for transactional queue operations
		/// </summary>
		/// <param name="transactionTimeout"></param>
		void SetDefaultTransactionTimeout(TimeSpan transactionTimeout);

		/// <summary>
		/// Sets the flag indicating that missing queues should be created
		/// </summary>
		/// <param name="createMissingQueues"></param>
		void SetCreateMissingQueues(bool createMissingQueues);

		/// <summary>
		/// When creating queues, attempt to create transactional queues if available
		/// </summary>
		/// <param name="createTransactionalQueues"></param>
		void SetCreateTransactionalQueues(bool createTransactionalQueues);

		/// <summary>
		/// Specifies if the input queue should be purged on startup
		/// </summary>
		/// <param name="purgeOnStartup"></param>
		void SetPurgeOnStartup(bool purgeOnStartup);

		/// <summary>
		/// Provides a configured endpoint builder for the specified URI
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="endpointBuilder"></param>
		void AddEndpointBuilder(Uri uri, EndpointBuilder endpointBuilder);

		/// <summary>
		/// Adds a transport factory to the builder
		/// </summary>
		void AddTransportFactory(ITransportFactory transportFactory);

        /// <summary>
        /// Add a message serializer to the builder so that messages with other serialization types
        /// can be received without conflicts
        /// </summary>
        /// <param name="serializer"></param>
	    void AddMessageSerializer(IMessageSerializer serializer);

		/// <summary>
		/// Sets the default isolation level for transports that perform transactional operations
		/// </summary>
		void SetDefaultIsolationLevel(IsolationLevel isolationLevel);

        /// <summary>
        /// Sets the default retry limit for inbound messages
        /// </summary>
	    void SetDefaultRetryLimit(int retryLimit);

        /// <summary>
        /// Sets the default message tracker factory for all endpoints
        /// </summary>
	    void SetDefaultInboundMessageTrackerFactory(MessageTrackerFactory messageTrackerFactory);

        /// <summary>
        /// Sets the supported message serializers for all endpoints
        /// </summary>
        void SetSupportedMessageSerializers(ISupportedMessageSerializers supportedSerializers);
	}
}