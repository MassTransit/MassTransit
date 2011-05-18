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
namespace MassTransit.Transports.Msmq
{
	using System;
	using System.Messaging;
	using System.Threading;
	using Context;
	using Exceptions;
	using log4net;
	using Magnum.Extensions;

	public abstract class InboundMsmqTransport :
		IInboundTransport
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (InboundMsmqTransport));
		static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Msmq.MessageLog");

		readonly IMsmqEndpointAddress _address;

		MessageQueueConnection _connection;
		bool _disposed;

		protected InboundMsmqTransport(IMsmqEndpointAddress address)
		{
			_address = address;
			_connection = new MessageQueueConnection(address, QueueAccessMode.Receive);
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public virtual void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
		{
			try
			{
				EnumerateQueue(callback, timeout);
			}
			catch (MessageQueueException ex)
			{
				HandleInboundMessageQueueException(ex, timeout);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected bool EnumerateQueue(Func<IReceiveContext, Action<IReceiveContext>> receiver, TimeSpan timeout)
		{
			if (_disposed)
				throw new ObjectDisposedException("The transport has been disposed: '{0}'".FormatWith(Address));

			bool received = false;

			using (MessageEnumerator enumerator = _connection.Queue.GetMessageEnumerator2())
			{
				if (_log.IsDebugEnabled)
					_log.DebugFormat("Enumerating endpoint: {0} ({1}ms)", Address, timeout);

				while (enumerator.MoveNext(timeout))
				{
					if (enumerator.Current == null)
					{
						if (_log.IsDebugEnabled)
							_log.DebugFormat("Current message was null while enumerating endpoint");

						continue;
					}

					IBusContext context;
					Action<IReceiveContext> receive;
					using (Message message = enumerator.Current)
					{
						context = new ConsumeContext(message.BodyStream);
						context.SetMessageId(message.Id);
						context.SetInputAddress(_address);

						var extension = message.Extension;
						if (extension.Length > 0)
						{
							var headers = TransportMessageHeaders.Create(extension);

							context.SetContentType(headers["Content-Type"]);
						}

						using(ContextStorage.CreateContextScope(context))
							{
								receive = receiver(context);
								if (receive == null)
								{
									if (_log.IsDebugEnabled)
										_log.DebugFormat("SKIP:{0}:{1}", Address, message.Id);

									if (_messageLog.IsDebugEnabled)
										_messageLog.DebugFormat("SKIP:{0}:{1}:{2}", _address.InboundFormatName, message.Label, message.Id);

									continue;
								}
							};
					}

					ReceiveMessage(enumerator, timeout, receiveCurrent =>
						{
							using (Message message = receiveCurrent())
							{
								if (message == null)
									throw new TransportException(Address.Uri,
										"Unable to remove message from queue: " + context.MessageId);

								if (message.Id != context.MessageId)
									throw new TransportException(Address.Uri,
										string.Format(
											"Received message does not match current message: ({0} != {1})",
											message.Id, context.MessageId));

								if (_messageLog.IsDebugEnabled)
									_messageLog.DebugFormat("RECV:{0}:{1}:{2}", _address.InboundFormatName, message.Label, message.Id);

								using (ContextStorage.CreateContextScope(context))
									receive(context);

								received = true;
							}
						});
				}
			}

			return received;
		}

		protected virtual void ReceiveMessage(MessageEnumerator enumerator, TimeSpan timeout,
		                                      Action<Func<Message>> receiveAction)
		{
			receiveAction(() => enumerator.RemoveCurrent(timeout, MessageQueueTransactionType.None));
		}

		protected void HandleInboundMessageQueueException(MessageQueueException ex, TimeSpan timeout)
		{
			switch (ex.MessageQueueErrorCode)
			{
				case MessageQueueErrorCode.IOTimeout:
					break;

				case MessageQueueErrorCode.ServiceNotAvailable:
					if (_log.IsErrorEnabled)
						_log.Error("The message queuing service is not available, pausing for timeout period", ex);

					Thread.Sleep(timeout);
					_connection.Disconnect();
					break;

				case MessageQueueErrorCode.QueueNotAvailable:
				case MessageQueueErrorCode.AccessDenied:
				case MessageQueueErrorCode.QueueDeleted:
					if (_log.IsErrorEnabled)
						_log.Error("The message queue was not available: " + _address.InboundFormatName, ex);

					Thread.Sleep(timeout);
					_connection.Disconnect();
					break;

				case MessageQueueErrorCode.QueueNotFound:
				case MessageQueueErrorCode.IllegalFormatName:
				case MessageQueueErrorCode.MachineNotFound:
					if (_log.IsErrorEnabled)
						_log.Error("The message queue was not found or is improperly named: " + _address.InboundFormatName, ex);

					Thread.Sleep(timeout);
					_connection.Disconnect();
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
							_address.InboundFormatName, ex);


					Thread.Sleep(timeout);
					_connection.Disconnect();
					break;

				default:
					if (_log.IsErrorEnabled)
						_log.Error("There was a problem communicating with the message queue: " + _address.InboundFormatName, ex);
					break;
			}
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_connection.Dispose();
				_connection = null;
			}

			_disposed = true;
		}

		~InboundMsmqTransport()
		{
			Dispose(false);
		}
	}
}