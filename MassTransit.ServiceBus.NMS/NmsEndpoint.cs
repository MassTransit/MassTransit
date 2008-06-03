/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.NMS
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using Apache.NMS;
	using Apache.NMS.ActiveMQ;
	using Exceptions;
	using log4net;

	public class NmsEndpoint :
		INmsEndpoint
	{
		private static readonly IFormatter _formatter = new BinaryFormatter();
		private static readonly ILog _log = LogManager.GetLogger(typeof (NmsMessageSender));
		private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Messages");
		private readonly IConnectionFactory _factory;
		private readonly string _queueName;
		private readonly Uri _uri;

		public NmsEndpoint(Uri uri)
		{
			_uri = uri;

			UriBuilder queueUri = new UriBuilder("tcp", Uri.Host, Uri.Port);

			_queueName = Uri.AbsolutePath.Substring(1);

			_factory = new ConnectionFactory(queueUri.Uri);
		}

		public NmsEndpoint(string uriString)
			: this(new Uri(uriString))
		{
		}

		public Uri Uri
		{
			get { return _uri; }
		}

		public void Send<T>(T message) where T : class
		{
			throw new NotImplementedException();
		}

		public void Send<T>(T message, TimeSpan timeToLive) where T : class
		{
			Type messageType = typeof (T);

			using (IConnection connection = _factory.CreateConnection())
			{
				using (ISession session = connection.CreateSession())
				{
					IDestination destination = session.GetQueue(_queueName);

					IBytesMessage bm = session.CreateBytesMessage();

					MemoryStream mem = new MemoryStream();
					_formatter.Serialize(mem, message);

					bm.Content = new byte[mem.Length];
					mem.Seek(0, SeekOrigin.Begin);
					mem.Read(bm.Content, 0, (int) mem.Length);

					if (timeToLive < NMSConstants.defaultTimeToLive)
						bm.NMSTimeToLive = timeToLive;

					bm.NMSPersistent = true;

					using (IMessageProducer producer = session.CreateProducer(destination))
					{
						try
						{
							if (_messageLog.IsInfoEnabled)
								_messageLog.InfoFormat("Message {0} Sent To {1}", messageType, Uri);

							producer.Send(bm);
						}
						catch (Exception ex)
						{
							throw new EndpointException(this, "Problem with " + Uri, ex);
						}
					}

					if (_log.IsDebugEnabled)
						_log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", bm.NMSMessageId, messageType);
				}
			}
		}

		public object Receive()
		{
			throw new NotImplementedException();
		}

		public object Receive(TimeSpan timeout)
		{
			throw new NotImplementedException();
		}

		public object Receive(Predicate<object> accept)
		{
			throw new NotImplementedException();
		}

		public object Receive(TimeSpan timeout, Predicate<object> accept)
		{
			throw new NotImplementedException();
		}

		public T Receive<T>() where T : class
		{
			throw new NotImplementedException();
		}

		public T Receive<T>(TimeSpan timeout) where T : class
		{
			throw new NotImplementedException();
		}

		public T Receive<T>(Predicate<T> accept) where T : class
		{
			throw new NotImplementedException();
		}

		public T Receive<T>(TimeSpan timeout, Predicate<T> accept) where T : class
		{
			throw new NotImplementedException();
		}


		public void Dispose()
		{
		}
	}
}