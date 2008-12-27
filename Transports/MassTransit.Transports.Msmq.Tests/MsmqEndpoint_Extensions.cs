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
namespace MassTransit.Transports.Msmq.Tests
{
	using System;
	using System.Messaging;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Transactions;
	using MassTransit.Tests;

	public static class MsmqEndpoint_Extensions
	{
		public static void Purge(this MsmqEndpoint ep)
		{
			using (MessageQueue queue = new MessageQueue(ep.QueuePath, QueueAccessMode.ReceiveAndAdmin))
			{
				queue.Purge();
			}
		}

		public static void VerifyMessageInQueue<T>(this MsmqEndpoint ep)
		{
			using (MessageQueue mq = new MessageQueue(ep.QueuePath, QueueAccessMode.Receive))
			{
				Message msg = mq.Receive(TimeSpan.FromSeconds(3));
				msg.ShouldNotBeNull();

				object message = new BinaryFormatter().Deserialize(msg.BodyStream);

				message.ShouldNotBeNull();

				message.ShouldBeSameType<T>();
			}
		}

		public static void VerifyMessageInTransactionalQueue<T>(this MsmqEndpoint ep)
		{
			using (TransactionScope trx = new TransactionScope())
			{
				ep.VerifyMessageInQueue<T>();
				trx.Complete();
			}
		}

		public static void VerifyMessageNotInTransactionalQueue(this MsmqEndpoint ep)
		{
			using (TransactionScope trx = new TransactionScope())
			{
				ep.VerifyMessageNotInQueue();
				trx.Complete();
			}
		}

		public static void VerifyMessageNotInQueue(this MsmqEndpoint ep)
		{
			using (MessageQueue mq = new MessageQueue(ep.QueuePath, QueueAccessMode.Receive))
			{
				try
				{
					Message msg = mq.Receive(TimeSpan.FromSeconds(0.1));
					msg.ShouldNotBeNull();
				}
				catch (MessageQueueException ex)
				{
					ex.Message
						.ShouldEqual("Timeout for the requested operation has expired.");
				}
			}
		}
	}
}