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
namespace MassTransit.Transports.Nms
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using Apache.NMS;
	using Apache.NMS.ActiveMQ;
	using Context;
	using Exceptions;
	using log4net;
	using Magnum;
	using Util;

	[DebuggerDisplay("{Address}")]
	public class NmsTransport :
		IDuplexTransport
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (NmsTransport));

		readonly IEndpointAddress _address;
		IConnection _connection;
		bool _disposed;
		IConnectionFactory _factory;

		public NmsTransport(IEndpointAddress address)
		{
			_address = address;

			Initialize();
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}


		public void Send(ISendContext context)
		{
			if (_disposed) throw NewDisposedException();

			try
			{
				using (ISession session = _connection.CreateSession(AcknowledgementMode.Transactional))
				{
					IQueue destination = session.GetQueue(Address.Path);

					using (IMessageProducer producer = session.CreateProducer(destination))
					{
						producer.DeliveryMode = MsgDeliveryMode.Persistent;

						string messageBody;
						using (var body = new MemoryStream())
						{
							context.SerializeTo(body);

							messageBody = Encoding.UTF8.GetString(body.ToArray());
						}

						ITextMessage message = producer.CreateTextMessage(messageBody);
						message.NMSDeliveryMode = MsgDeliveryMode.Persistent;
						message.Properties["message-type"] = context.MessageType;

						if (context.ExpirationTime.HasValue)
						{
							DateTime value = context.ExpirationTime.Value;
							message.NMSTimeToLive = value.Kind == DateTimeKind.Utc ? value - SystemUtil.UtcNow : value - SystemUtil.Now;
						}

						producer.Send(message);

						session.Commit();

						if (SpecialLoggers.Messages.IsInfoEnabled)
							SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}", Address, message.NMSMessageId);
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

		public void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
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
									var context = new ConsumeContext(bodyStream);
									using (ContextStorage.CreateContextScope(context))
									{
										context.SetMessageId(textMessage.NMSMessageId);
										context.SetInputAddress(_address);

										Action<IReceiveContext> receive = callback(context);
										if (receive == null)
										{
											if (_log.IsDebugEnabled)
												_log.DebugFormat("SKIP:{0}:{1}", Address, message.NMSMessageId);

											if (SpecialLoggers.Messages.IsInfoEnabled)
												SpecialLoggers.Messages.InfoFormat("SKIP:{0}:{1}", Address, message.NMSMessageId);
										}
										else
										{
											receive(context);
										}
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


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IOutboundTransport OutboundTransport
		{
			get { return this; }
		}

		public IInboundTransport InboundTransport
		{
			get { return this; }
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				Disconnect();
			}

			_disposed = true;
		}

		Uri GenerateServerUri()
		{
			return new UriBuilder("tcp", Address.Uri.Host, Address.Uri.Port).Uri;
		}

		void Initialize()
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

		void Connect()
		{
			Disconnect();

			_connection = _factory.CreateConnection();
			_connection.ExceptionListener += ConnectionExceptionListener;

			_connection.Start();
		}

		void Disconnect()
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

		void Reconnect()
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

		void ConnectionExceptionListener(Exception ex)
		{
			_log.Error("An exception occurred on the endpoint: " + Address, ex);

			Reconnect();
		}

		ObjectDisposedException NewDisposedException()
		{
			return new ObjectDisposedException("The transport has already been disposed: " + Address);
		}

		~NmsTransport()
		{
			Dispose(false);
		}
	}
}