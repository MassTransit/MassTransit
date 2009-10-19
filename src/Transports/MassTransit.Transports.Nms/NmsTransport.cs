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
namespace MassTransit.Transports.Nms
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using Apache.NMS;
	using Apache.NMS.ActiveMQ;
	using Exceptions;
	using Internal;
	using log4net;

	[DebuggerDisplay("{Address}")]
	public class NmsTransport :
		ITransport
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (NmsTransport));

		private readonly IEndpointAddress _address;
		private IConnection _connection;
		private bool _disposed;
		private IConnectionFactory _factory;

		public NmsTransport(IEndpointAddress address)
		{
			_address = address;

			Initialize();
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public virtual void Receive(Func<Stream, Action<Stream>> receiver)
		{
			if (_disposed) throw NewDisposedException();

			Receive(receiver, TimeSpan.Zero);
		}

		public virtual void Receive(Func<Stream, Action<Stream>> receiver, TimeSpan timeout)
		{
			if (_disposed) throw NewDisposedException();

			try
			{
				using (ISession session = _connection.CreateSession(AcknowledgementMode.Transactional))
				{
					IQueue destination = session.GetQueue(Address.Path);

					using (IMessageConsumer consumer = session.CreateConsumer(destination))
					{
						IMessage message = consumer.Receive(timeout);
						if (message == null)
						{
							if (_log.IsDebugEnabled)
								_log.DebugFormat("NULL:{0}", Address);

							if (SpecialLoggers.Messages.IsInfoEnabled)
								SpecialLoggers.Messages.InfoFormat("NULL:{0}", Address);
						}
						else
						{
							var textMessage = message as ITextMessage;
							if (textMessage == null)
							{
								if (_log.IsDebugEnabled)
									_log.DebugFormat("UNKN:{0}:{1}", Address, message.GetType());

								if (SpecialLoggers.Messages.IsInfoEnabled)
									SpecialLoggers.Messages.InfoFormat("NULL:{0}:{1}", Address, message.GetType());
							}
							else
							{
								using (var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(textMessage.Text), false))
								{
									Action<Stream> receive = receiver(bodyStream);
									if (receive == null)
									{
										if (_log.IsDebugEnabled)
											_log.DebugFormat("SKIP:{0}:{1}", Address, message.NMSMessageId);

										if (SpecialLoggers.Messages.IsInfoEnabled)
											SpecialLoggers.Messages.InfoFormat("SKIP:{0}:{1}", Address, message.NMSMessageId);
									}
									else
									{
										receive(bodyStream);
									}
								}
							}
						}
					}

					session.Commit();
				}
			}
			catch (InvalidOperationException ex)
			{
				if (_log.IsErrorEnabled)
					_log.Error("A problem occurred communicating with the queue", ex);

				Reconnect();
			}
		}

		public virtual void Send(Action<Stream> sender)
		{
			if (_disposed) throw NewDisposedException();

			try
			{
				using (ISession session = _connection.CreateSession(AcknowledgementMode.Transactional))
				{
					IQueue destination = session.GetQueue(Address.Path);

					using (IMessageProducer producer = session.CreateProducer(destination))
					{
						using (var bodyStream = new MemoryStream())
						{
							sender(bodyStream);

							string text = Encoding.UTF8.GetString(bodyStream.ToArray());

							ITextMessage textMessage = session.CreateTextMessage(text);

							SetMessageExpiration(textMessage);

							producer.DeliveryMode = MsgDeliveryMode.Persistent;
							producer.Send(textMessage);

							session.Commit();

							if (SpecialLoggers.Messages.IsInfoEnabled)
								SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}", Address, textMessage.NMSMessageId);
						}
					}
				}
			}
			catch (InvalidOperationException ex)
			{
				if (_log.IsErrorEnabled)
					_log.Error("A problem occurred communicating with the queue", ex);

				Reconnect();
			}
		}

		private static void SetMessageExpiration(IMessage outbound)
		{
			if (OutboundMessage.Headers.ExpirationTime.HasValue)
			{
				outbound.NMSTimeToLive = OutboundMessage.Headers.ExpirationTime.Value - DateTime.UtcNow;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				Disconnect();
			}

			_disposed = true;
		}

		private Uri GenerateServerUri()
		{
			return new UriBuilder("tcp", Address.Uri.Host, Address.Uri.Port).Uri;
		}

		private void Initialize()
		{
			try
			{
				_factory = new ConnectionFactory(GenerateServerUri());

				Connect();
			}
			catch (Exception ex)
			{
				throw new TransportException(Address.Uri, "Failed to initialize transport", ex);
			}
		}

		private void Connect()
		{
			Disconnect();

			_connection = _factory.CreateConnection();
			_connection.ExceptionListener += ConnectionExceptionListener;

			_connection.Start();
		}

		private void Disconnect()
		{
			if (_connection == null) return;

			try
			{
				_connection.Stop();
				_connection.ExceptionListener -= ConnectionExceptionListener;
				_connection.Close();
				_connection.Dispose();
				_connection = null;
			}
			catch (Exception ex)
			{
				_log.Error("Failed to close existing connection", ex);
			}
		}

		private void Reconnect()
		{
			try
			{
				Disconnect();
			}
			catch (Exception ex)
			{
				_log.Error("Failed to disconnect from " + Address, ex);
			}
			finally
			{
				Connect();
			}
		}

		private void ConnectionExceptionListener(Exception ex)
		{
			_log.Error("An exception occurred on the endpoint: " + Address, ex);

			Reconnect();
		}

		private ObjectDisposedException NewDisposedException()
		{
			return new ObjectDisposedException("The transport has already been disposed: " + Address);
		}

		~NmsTransport()
		{
			Dispose(false);
		}
	}
}