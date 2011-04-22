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
	using log4net;

	public abstract class OutboundMsmqTransport :
		IOutboundTransport
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (OutboundMsmqTransport));
		private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Msmq.MessageLog");
		private readonly IMsmqEndpointAddress _address;

		private MessageQueueConnection _connection;
		private bool _disposed;

		protected OutboundMsmqTransport(IMsmqEndpointAddress address)
		{
			_address = address;
			_connection = new MessageQueueConnection(address, QueueAccessMode.Send);
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public void Send(Action<ISendContext> callback)
		{
			using (var context = new MsmqSendContext())
			{
				callback(context);

				try
				{
					using (MessageQueue queue = _connection.Queue)
					{
						SendMessage(queue, context.Message);

						if (_messageLog.IsDebugEnabled)
							_messageLog.DebugFormat("SEND:{0}:{1}:{2}", _address.OutboundFormatName, context.Message.Label, context.Message.Id);
					}
				}
				catch (MessageQueueException ex)
				{
					HandleOutboundMessageQueueException(ex);
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

		private void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_connection.Dispose();
				_connection = null;
			}

			_disposed = true;
		}

		private void HandleOutboundMessageQueueException(MessageQueueException ex)
		{
			_connection.Disconnect();

			switch (ex.MessageQueueErrorCode)
			{
				case MessageQueueErrorCode.IOTimeout:
					break;

				case MessageQueueErrorCode.ServiceNotAvailable:
					if (_log.IsErrorEnabled)
						_log.Error("The message queuing service is not available, pausing for timeout period", ex);
					break;

				case MessageQueueErrorCode.QueueNotAvailable:
				case MessageQueueErrorCode.AccessDenied:
				case MessageQueueErrorCode.QueueDeleted:
					if (_log.IsErrorEnabled)
						_log.Error("The message queue was not available: " + _connection.FormatName, ex);
					break;

				case MessageQueueErrorCode.QueueNotFound:
				case MessageQueueErrorCode.IllegalFormatName:
				case MessageQueueErrorCode.MachineNotFound:
					if (_log.IsErrorEnabled)
						_log.Error("The message queue was not found or is improperly named: " + _address.InboundFormatName, ex);
					break;

				case MessageQueueErrorCode.InvalidHandle:
				case MessageQueueErrorCode.StaleHandle:
					if (_log.IsErrorEnabled)
						_log.Error(
							"The message queue handle is stale or no longer valid due to a restart of the message queuing service: " +
							_address.InboundFormatName, ex);
					break;

				default:
					if (_log.IsErrorEnabled)
						_log.Error("There was a problem communicating with the message queue: " + _address.InboundFormatName, ex);
					break;
			}
		}

		~OutboundMsmqTransport()
		{
			Dispose(false);
		}
	}
}