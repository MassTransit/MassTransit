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
	using System.Diagnostics;
	using System.IO;
	using System.Messaging;
	using System.Threading;
	using Exceptions;
	using Internal;
	using log4net;
	using Magnum.DateTimeExtensions;

	[DebuggerDisplay("{Address}")]
	public class AbstractMsmqTransport :
		IMsmqTransport
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (NonTransactionalMsmqTransport));
		private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Msmq.MessageLog");
		private IMsmqEndpointAddress _address;
		private bool _disposed;
		private MessageQueue _queue;

		protected AbstractMsmqTransport(IMsmqEndpointAddress address)
		{
			_address = address;
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public IMsmqEndpointAddress MsmqAddress
		{
			get { return _address; }
		}

		public virtual void Receive(Func<Message, Action<Message>> receiver)
		{
			if (_disposed) throw NewDisposedException();

			Receive(receiver, TimeSpan.Zero);
		}

		public virtual void Receive(Func<Message, Action<Message>> receiver, TimeSpan timeout)
		{
			try
			{
				Connect();

				EnumerateQueue(receiver, timeout);
			}
			catch (MessageQueueException ex)
			{
				HandleMessageQueueException(ex, timeout);
			}
		}

		protected virtual bool EnumerateQueue(Func<Message, Action<Message>> receiver, TimeSpan timeout)
		{
			if (_disposed) throw NewDisposedException();

			bool received = false;

			using (MessageEnumerator enumerator = _queue.GetMessageEnumerator2())
			{
				if (_log.IsDebugEnabled)
					_log.DebugFormat("Enumerating endpoint: {0} ({1}ms)", Address, timeout);

				while (enumerator.MoveNext(timeout))
				{
					Message current = enumerator.Current;
					if (current == null)
					{
						if (_log.IsDebugEnabled)
							_log.DebugFormat("Current message was null while enumerating endpoint");

						continue;
					}

					Action<Message> receive = receiver(current);
					if (receive == null)
					{
						if (_log.IsDebugEnabled)
							_log.DebugFormat("SKIP:{0}:{1}", Address, current.Id);

						if (SpecialLoggers.Messages.IsInfoEnabled)
							SpecialLoggers.Messages.InfoFormat("SKIP:{0}:{1}", Address, current.Id);

						continue;
					}

					ReceiveMessage(enumerator, timeout, receiveCurrent =>
						{
							using (Message message = receiveCurrent())
							{
								if (message == null)
									throw new TransportException(Address.Uri, "Unable to remove message from queue: " + current.Id);

								if (message.Id != current.Id)
									throw new TransportException(Address.Uri,
										string.Format("Received message does not match current message: ({0} != {1})", message.Id, current.Id));

								receive(message);

								received = true;
							}
						});
				}
			}

			return received;
		}

		public virtual void Send(Action<Message> sender)
		{
			if (_disposed) throw NewDisposedException();

			using (var message = new Message())
			{
				sender(message);

				try
				{
					using (var queue = new MessageQueue(_address.FormatName, QueueAccessMode.Send))
					{
						SendMessage(queue, message);

						if (_messageLog.IsDebugEnabled)
							_messageLog.DebugFormat("SEND:{0}:{1}", Address, message.Id);
					}
				}
				catch (MessageQueueException ex)
				{
					HandleMessageQueueException(ex, 2.Seconds());
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Receive(Func<Stream, Action<Stream>> receiver)
		{
			if (_disposed) throw NewDisposedException();

			Receive(receiver, TimeSpan.Zero);
		}

		public virtual void Receive(Func<Stream, Action<Stream>> receiver, TimeSpan timeout)
		{
			if (_disposed) throw NewDisposedException();

			Receive(ReceiveAsStream(receiver), timeout);
		}

		public virtual void Send(Action<Stream> sender)
		{
			if (_disposed) throw NewDisposedException();

			Action<Message> messageSender = m => sender(m.BodyStream);

			Send(messageSender);
		}

		protected virtual void SendMessage(MessageQueue queue, Message message)
		{
			queue.Send(message, MessageQueueTransactionType.None);
		}

		protected virtual void ReceiveMessage(MessageEnumerator enumerator, TimeSpan timeout, Action<Func<Message>> receiveAction)
		{
			receiveAction(() => enumerator.RemoveCurrent(timeout, MessageQueueTransactionType.None));
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				Disconnect();

				_address = null;
			}

			_disposed = true;
		}

		private ObjectDisposedException NewDisposedException()
		{
			return new ObjectDisposedException("The transport has already been disposed: " + Address);
		}

		~AbstractMsmqTransport()
		{
			Dispose(false);
		}

		private static Func<Message, Action<Message>> ReceiveAsStream(Func<Stream, Action<Stream>> receiver)
		{
			return message =>
				{
					Action<Stream> receive = receiver(message.BodyStream);
					if (receive == null)
						return null;

					return m => receive(m.BodyStream);
				};
		}

		protected void HandleMessageQueueException(MessageQueueException ex, TimeSpan timeout)
		{
			switch (ex.MessageQueueErrorCode)
			{
				case MessageQueueErrorCode.IOTimeout:
					break;

				case MessageQueueErrorCode.ServiceNotAvailable:
					if (_log.IsErrorEnabled)
						_log.Error("The message queuing service is not available, pausing for timeout period", ex);

					Thread.Sleep(timeout);
					Reconnect();
					break;

				case MessageQueueErrorCode.QueueNotAvailable:
				case MessageQueueErrorCode.AccessDenied:
				case MessageQueueErrorCode.QueueDeleted:
					if (_log.IsErrorEnabled)
						_log.Error("The message queue was not available: " + _address.FormatName, ex);

					Thread.Sleep(timeout);
					Reconnect();
					break;

				case MessageQueueErrorCode.QueueNotFound:
				case MessageQueueErrorCode.IllegalFormatName:
				case MessageQueueErrorCode.MachineNotFound:
					if (_log.IsErrorEnabled)
						_log.Error("The message queue was not found or is improperly named: " + _address.FormatName, ex);

					Thread.Sleep(timeout);
					Reconnect();
					break;

				case MessageQueueErrorCode.MessageAlreadyReceived:
					// we are competing with another consumer, no reason to report an error since
					// the message has already been handled.
					if (_log.IsDebugEnabled)
						_log.Debug("The message was removed from the queue before it could be received. This could be the result of another service reading from the same queue.");
					break;

				case MessageQueueErrorCode.InvalidHandle:
				case MessageQueueErrorCode.StaleHandle:
					if (_log.IsErrorEnabled)
						_log.Error("The message queue handle is stale or no longer valid due to a restart of the message queuing service: " + _address.FormatName, ex);

					Reconnect();

					Thread.Sleep(timeout);
					break;

				default:
					if (_log.IsErrorEnabled)
						_log.Error("There was a problem communicating with the message queue: " + _address.FormatName, ex);
					break;
			}
		}

		protected virtual void Connect()
		{
			if (_queue != null)
				return;

			_queue = new MessageQueue(_address.FormatName, QueueAccessMode.Receive);
		}

		protected virtual void Disconnect()
		{
			if (_queue != null)
			{
				_queue.Dispose();
				_queue = null;
			}
		}

		protected virtual void Reconnect()
		{
			Disconnect();
			Connect();
		}
	}
}