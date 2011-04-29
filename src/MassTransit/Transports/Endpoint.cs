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
namespace MassTransit.Transports
{
	using System;
	using System.Diagnostics;
	using System.Runtime.Serialization;
	using log4net;
	using MessageHeaders;
	using Serialization;
	using Util;

	[DebuggerDisplay("{Address}")]
	public class Endpoint :
		IEndpoint
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (Endpoint));
		static IEndpoint _null;
		readonly IEndpointAddress _address;
		readonly IMessageSerializer _serializer;
		readonly MessageRetryTracker _tracker;
		bool _disposed;
		string _disposedMessage;
		IOutboundTransport _errorTransport;
		IDuplexTransport _transport;

		public Endpoint(IEndpointAddress address, IMessageSerializer serializer, IDuplexTransport transport,
		                IOutboundTransport errorTransport)
		{
			_address = address;
			_transport = transport;
			_errorTransport = errorTransport;
			_serializer = serializer;

			_tracker = new MessageRetryTracker(5);

			SetDisposedMessage();
		}

		public static IEndpoint Null
		{
			get { return _null ?? (_null = CreateNullEndpoint()); }
		}

		public IOutboundTransport ErrorTransport
		{
			get { return _errorTransport; }
		}

		public IMessageSerializer Serializer
		{
			get { return _serializer; }
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public Uri Uri
		{
			get { return Address.Uri; }
		}

		public IInboundTransport InboundTransport
		{
			get { return _transport.InboundTransport; }
		}

		public IOutboundTransport OutboundTransport
		{
			get { return _transport.OutboundTransport; }
		}

		public void Send<T>(T message)
			where T : class
		{
			if (_disposed) throw NewDisposedException();

			_transport.Send(context =>
				{
					SetOutboundMessageHeaders<T>();

					_serializer.Serialize(context.Body, message);

					context.SetLabel(typeof (T).Name);
					context.MarkRecoverable();

					if (OutboundMessage.Headers.ExpirationTime.HasValue)
						context.SetMessageExpiration(OutboundMessage.Headers.ExpirationTime.Value);

					if (SpecialLoggers.Messages.IsInfoEnabled)
						SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}", Address, typeof (T).Name);
				});
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Receive(Func<object, Action<object>> receiver, TimeSpan timeout)
		{
			if (_disposed) throw NewDisposedException();

			_transport.Receive(ReceiveFromTransport(receiver), timeout);
		}

		void SetDisposedMessage()
		{
			_disposedMessage = "The endpoint has already been disposed: " + _address;
		}

		ObjectDisposedException NewDisposedException()
		{
			return new ObjectDisposedException(_disposedMessage);
		}

		void SetOutboundMessageHeaders<T>()
		{
			OutboundMessage.Set(headers =>
				{
					headers.SetMessageType(typeof (T));
					headers.SetDestinationAddress(Uri);
				});
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_tracker.Dispose();

				_transport.Dispose();
				_transport = null;

				_errorTransport.Dispose();
				_errorTransport = null;
			}

			_disposed = true;
		}

		void MoveMessageToErrorTransport(IReceiveContext message)
		{
			_errorTransport.Send(context =>
				{
					message.Body.CopyTo(context.Body);
					if (OutboundMessage.Headers.ExpirationTime.HasValue)
						context.SetMessageExpiration(OutboundMessage.Headers.ExpirationTime.Value);
				});

			if (_log.IsDebugEnabled)
				_log.DebugFormat("MOVE:{0}:{1}:{2}", Address, _errorTransport.Address, message.MessageId);

			if (SpecialLoggers.Messages.IsInfoEnabled)
				SpecialLoggers.Messages.InfoFormat("MOVE:{0}:{1}:{2}", Address, _errorTransport.Address, message.MessageId);
		}

		Func<IReceiveContext, Action<IReceiveContext>> ReceiveFromTransport(Func<object, Action<object>> receiver)
		{
			return receivingContext =>
				{
					if (_tracker.IsRetryLimitExceeded(receivingContext.MessageId))
					{
						if (_log.IsErrorEnabled)
							_log.ErrorFormat("Message retry limit exceeded {0}:{1}", Address, receivingContext.MessageId);

						return MoveMessageToErrorTransport;
					}

					object messageObj;

					try
					{
						messageObj = _serializer.Deserialize(receivingContext.Body);
					}
					catch (SerializationException sex)
					{
						if (_log.IsErrorEnabled)
							_log.Error("Unrecognized message " + Address + ":" + receivingContext.MessageId, sex);

						_tracker.IncrementRetryCount(receivingContext.MessageId);
						return MoveMessageToErrorTransport;
					}

					if (messageObj == null)
						return null;

					Action<object> receive;
					try
					{
						receive = receiver(messageObj);
						if (receive == null)
						{
							if (_log.IsDebugEnabled)
								_log.DebugFormat("SKIP:{0}:{1}", Address, messageObj.GetType().Name);

							if (SpecialLoggers.Messages.IsInfoEnabled)
								SpecialLoggers.Messages.InfoFormat("SKIP:{0}:{1}:{2}", Address, messageObj.GetType().Name,
									receivingContext.MessageId);

							return null;
						}
					}
					catch (Exception ex)
					{
						if (_log.IsErrorEnabled)
							_log.Error("An exception was thrown preparing the message consumers", ex);

						_tracker.IncrementRetryCount(receivingContext.MessageId);
						return null;
					}

					return m =>
						{
							if (_log.IsDebugEnabled)
								_log.DebugFormat("RECV:{0}:{1}:{2}", Address, m.MessageId, messageObj.GetType().Name);

							if (SpecialLoggers.Messages.IsInfoEnabled)
								SpecialLoggers.Messages.InfoFormat("RECV:{0}:{1}:{2}", Address, messageObj.GetType().Name, m.MessageId);

							try
							{
								receive(messageObj);

								_tracker.MessageWasReceivedSuccessfully(m.MessageId);
							}
							catch (Exception ex)
							{
								if (_log.IsErrorEnabled)
									_log.Error("An exception was thrown by a message consumer", ex);

								_tracker.IncrementRetryCount(m.MessageId);
								MoveMessageToErrorTransport(m);
							}
						};
				};
		}

		~Endpoint()
		{
			Dispose(false);
		}

		static Endpoint CreateNullEndpoint()
		{
			return new Endpoint(EndpointAddress.Null, new XmlMessageSerializer(), new NullTransport(EndpointAddress.Null),
				new NullTransport(EndpointAddress.Null));
		}
	}
}