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
	using System.Transactions;
	using log4net;
	using Magnum.DateTimeExtensions;

	public class TransactionalMsmqTransport :
		AbstractMsmqTransport
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (TransactionalMsmqTransport));

		public TransactionalMsmqTransport(IMsmqEndpointAddress address)
			: base(address)
		{
		}

		public override void Receive(Func<Message, Action<Message>> receiver, TimeSpan timeout)
		{
			try
			{
				Connect();

				var options = new TransactionOptions
					{
						IsolationLevel = IsolationLevel.Serializable,
						Timeout = 30.Seconds(),
					};

				using (var scope = new TransactionScope(TransactionScopeOption.Required, options))
				{
					if (EnumerateQueue(receiver, timeout))
						scope.Complete();
				}
			}
			catch (MessageQueueException ex)
			{
				HandleMessageQueueException(ex, timeout);
			}
		}

		protected override void ReceiveMessage(MessageEnumerator enumerator, TimeSpan timeout, Action<Func<Message>> receiveAction)
		{
			receiveAction(() =>
				{
					if (_log.IsDebugEnabled)
						_log.DebugFormat("Removing message {0} from queue {1}", enumerator.Current.Id, Address);

					return enumerator.RemoveCurrent(timeout, MessageQueueTransactionType.Automatic);
				});
		}

		protected override void SendMessage(MessageQueue queue, Message message)
		{
			var tt = (Transaction.Current != null) ? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.Single;

			queue.Send(message, tt);
		}
	}
}