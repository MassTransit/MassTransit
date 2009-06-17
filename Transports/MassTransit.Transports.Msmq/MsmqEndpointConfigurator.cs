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
	using System;
	using System.Messaging;
	using Configuration;
	using Exceptions;
	using Magnum;
	using Serialization;

	public class MsmqEndpointConfigurator :
		EndpointConfiguratorBase
	{
		private static MsmqEndpointConfiguratorDefaults _defaults = new MsmqEndpointConfiguratorDefaults();

		public static IEndpoint New(Action<IEndpointConfigurator> action)
		{
			var configurator = new MsmqEndpointConfigurator();

			action(configurator);

			return configurator.Create();
		}

		public static void Defaults(Action<IMsmqEndpointDefaults> configureDefaults)
		{
			configureDefaults(_defaults);
		}

		private IEndpoint Create()
		{
			Guard.Against.Null(Uri, "No Uri was specified for the endpoint");
			Guard.Against.Null(SerializerType, "No serializer type was specified for the endpoint");

			IMessageSerializer serializer = GetSerializer();

			if (_defaults.CreateMissingQueues)
				CreateQueueIfMissing();

			if (_defaults.PurgeOnStartup)
				PurgeQueue();

			var endpoint = new MsmqEndpoint(Uri, serializer);

			return endpoint;
		}

		private void PurgeQueue()
		{
			var queueAddress = new QueueAddress(Uri);

			MessageQueue queue = new MessageQueue(queueAddress.FormatName, QueueAccessMode.ReceiveAndAdmin);
			queue.Purge();
		}

		private void CreateQueueIfMissing()
		{
			var queueAddress = new QueueAddress(Uri);

			MessageQueue queue = new MessageQueue(queueAddress.FormatName, QueueAccessMode.ReceiveAndAdmin);
			if (!queue.CanRead)
			{
				if (!queueAddress.IsLocal)
					throw new EndpointException(Uri, "The endpoint does not exist and cannot be created because it is not local");

				queue = MessageQueue.Create(queueAddress.LocalName, _defaults.CreateTransactionalQueues);
			}

			if (!queue.CanRead)
				throw new EndpointException(Uri, "The endpoint could not be found or created: " + queueAddress.ActualUri);
		}
	}
}