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
	using System.Runtime.Serialization;
	using Exceptions;
	using Internal;
	using log4net;
	using Serialization;

	public class MsmqMessageSelector :
		IMessageSelector
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (MsmqMessageSelector));

		private readonly MsmqEndpoint _endpoint;
		private readonly MessageEnumerator _enumerator;
		private readonly IMessageSerializer _serializer;
		private volatile bool _disposed;
		private object _message;
		private MessageQueueTransactionType _receiveTransactionType;
		private TimeSpan _timeout;
		private Message _transportMessage;

		public MsmqMessageSelector(MsmqEndpoint endpoint, MessageEnumerator enumerator, IMessageSerializer serializer)
		{
			_endpoint = endpoint;
			_enumerator = enumerator;
			_serializer = serializer;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public bool AcceptMessage()
		{
			if (_disposed) throw new ObjectDisposedException("The object has been disposed");

			try
			{
				using (Message received = _enumerator.RemoveCurrent(_timeout, _endpoint.ReceiveTransactionType))
				{
					if (received == null)
						throw new MessageException(_message.GetType(), "The message could not be removed from the queue");

					if (received.Id != _transportMessage.Id)
						throw new MessageException(_message.GetType(), "The message removed does not match the original message");
				}

				if (_log.IsDebugEnabled)
					_log.DebugFormat("Received {0} from {1} [{2}]", _message.GetType().FullName, _endpoint.Uri, _transportMessage.Id);

				if (SpecialLoggers.Messages.IsInfoEnabled)
					SpecialLoggers.Messages.InfoFormat("RECV:{0}:{1}", _endpoint.Uri, _message.GetType());

				return true;
			}
			catch (Exception ex)
			{
				_log.Error("An error occurred removing the message from the queue " + _endpoint.Uri, ex);
				return false;
			}
		}

		public void MoveMessageTo(IEndpoint endpoint)
		{
			endpoint.Send(_message);
		}

		public object DeserializeMessage()
		{
			if (_disposed) throw new ObjectDisposedException("The object has been disposed");

			try
			{
				_transportMessage = _enumerator.Current;

				if (_transportMessage != null)
				{
					_message = _serializer.Deserialize(_transportMessage.BodyStream);

					return _message;
				}

				throw new EndpointException(_endpoint, "Unable to retrieve current message from queue");
			}
				catch(MessageQueueException mqex)
				{
					if (mqex.MessageQueueErrorCode == MessageQueueErrorCode.MessageAlreadyReceived)
						return null;

					throw;
				}
			catch (SerializationException ex)
			{
				// if we get a message we cannot serialize, we need to do something about it or it will 
				// hang the service bus forever

				_endpoint.DiscardMessage(_transportMessage.Id, ex.Message);

				throw new MessageException(typeof (object), "An error occurred deserializing a message", ex);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (_transportMessage != null)
				{
					_transportMessage.Dispose();
					_transportMessage = null;
				}
			}
			_disposed = true;
		}

		~MsmqMessageSelector()
		{
			Dispose(false);
		}
	}
}