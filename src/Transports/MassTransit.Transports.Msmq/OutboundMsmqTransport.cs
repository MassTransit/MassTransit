// Copyright 2007-2011 The Apache Software Foundation.
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
	using System.Threading;
	using log4net;
	using Magnum.Extensions;

	public abstract class OutboundMsmqTransport :
		IOutboundTransport
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (OutboundMsmqTransport));
		private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Msmq.MessageLog");
		private readonly IMsmqEndpointAddress _address;
		private bool _disposed;

		protected OutboundMsmqTransport(IMsmqEndpointAddress address)
		{
			_address = address;
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}


		public void Send(Action<ISendContext> callback)
		{
			if (_disposed) throw NewDisposedException();

			using (var context = new MsmqSendContext())
			{
				callback(context);

				try
				{
					using (var queue = new MessageQueue(_address.FormatName, QueueAccessMode.Send))
					{
						SendMessage(queue, context.Message);

						if (_messageLog.IsDebugEnabled)
							_messageLog.DebugFormat("SEND:{0}:{1}", Address, context.Message.Id);
					}
				}
				catch (MessageQueueException ex)
				{
					HandleOutboundMessageQueueException(ex, 2.Seconds());
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void SendMessage(MessageQueue queue, Message message)
		{
			queue.Send(message, MessageQueueTransactionType.None);
		}

		protected ObjectDisposedException NewDisposedException()
		{
			return new ObjectDisposedException("The transport has already been disposed: '{0}'".FormatWith(Address));
		}

		protected void HandleOutboundMessageQueueException(MessageQueueException ex, TimeSpan timeout)
		{
			switch (ex.MessageQueueErrorCode)
			{
				case MessageQueueErrorCode.IOTimeout:
					break;

				case MessageQueueErrorCode.ServiceNotAvailable:
					if (_log.IsErrorEnabled)
						_log.Error("The message queuing service is not available, pausing for timeout period", ex);

					Thread.Sleep(timeout);
					break;

				case MessageQueueErrorCode.QueueNotAvailable:
				case MessageQueueErrorCode.AccessDenied:
				case MessageQueueErrorCode.QueueDeleted:
					if (_log.IsErrorEnabled)
						_log.Error("The message queue was not available: " + _address.FormatName, ex);

					Thread.Sleep(timeout);
					break;

				case MessageQueueErrorCode.QueueNotFound:
				case MessageQueueErrorCode.IllegalFormatName:
				case MessageQueueErrorCode.MachineNotFound:
					if (_log.IsErrorEnabled)
						_log.Error("The message queue was not found or is improperly named: " + _address.FormatName, ex);

					Thread.Sleep(timeout);
					break;

				case MessageQueueErrorCode.MessageAlreadyReceived:
					// we are competing with another consumer, no reason to report an error since
					// the message has already been handled.
					if (_log.IsDebugEnabled)
						_log.Debug(
							"The message was removed from the queue before it could be received. This could be the result of another service reading from the same queue.");
					break;

				case MessageQueueErrorCode.InvalidHandle:
				case MessageQueueErrorCode.StaleHandle:
					if (_log.IsErrorEnabled)
						_log.Error(
							"The message queue handle is stale or no longer valid due to a restart of the message queuing service: " +
							_address.FormatName, ex);


					Thread.Sleep(timeout);
					break;

				default:
					if (_log.IsErrorEnabled)
						_log.Error("There was a problem communicating with the message queue: " + _address.FormatName, ex);
					break;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
			}

			_disposed = true;
		}

		~OutboundMsmqTransport()
		{
			Dispose(false);
		}
	}
}