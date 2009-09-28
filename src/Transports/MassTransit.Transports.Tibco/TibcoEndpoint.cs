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
namespace MassTransit.Transports.Tibco
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using Configuration;
	using Exceptions;
	using Internal;
	using log4net;
	using Serialization;
	using TIBCO.EMS;

	public class TibcoEndpoint :
		IEndpoint,
		IExceptionListener
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (TibcoEndpoint));

		private readonly IMessageSerializer _serializer;
		private readonly Uri _uri;
		private Connection _connection;
		private int _deliveryMode = DeliveryMode.RELIABLE_DELIVERY;
		private bool _disposed;
		private ConnectionFactory _factory;
		private string _queueName;

		public TibcoEndpoint(string uriString, IMessageSerializer serializer)
			: this(new Uri(uriString), serializer)
		{
		}

		public TibcoEndpoint(Uri uri, IMessageSerializer serializer)
		{
			_uri = uri;
			_serializer = serializer;

			_queueName = Uri.AbsolutePath.Substring(1);

			string url = _uri.Host + ":" + _uri.Port;

			_factory = new ConnectionFactory(url);

			_connection = _factory.CreateConnection();

			_connection.ExceptionListener = this;

			_connection.Start();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public Uri Uri
		{
			get { return _uri; }
		}

		public void Send<T>(T message) where T : class
		{
			Send(message, TimeSpan.MaxValue);
		}

		public void Send<T>(T message, TimeSpan timeToLive) where T : class
		{
			if (_disposed) throw new ObjectDisposedException("The object has been disposed");

			Session session = _connection.CreateSession(true, SessionMode.SessionTransacted);
			if (session == null)
				throw new EndpointException(Uri, "Unable to open session to endpoint");

			Destination destination = session.CreateQueue(_queueName);

			MessageProducer producer = session.CreateProducer(destination);
			if (producer != null)
			{
				Type messageType = typeof (T);

				TextMessage textMessage = session.CreateTextMessage();

				using (MemoryStream mem = new MemoryStream())
				{
					_serializer.Serialize(mem, message);

					textMessage.Text = Encoding.UTF8.GetString(mem.ToArray());
				}

				if (timeToLive < TimeSpan.MaxValue)
					producer.TimeToLive = (long) timeToLive.TotalMilliseconds;

				producer.DeliveryMode = _deliveryMode;

				producer.Send(textMessage);

				session.Commit();

				if (SpecialLoggers.Messages.IsInfoEnabled)
					SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}:{2}", Uri, messageType.Name, textMessage.MessageID);

				if (_log.IsDebugEnabled)
					_log.DebugFormat("Sent {0} from {1} [{2}]", messageType.FullName, Uri, textMessage.MessageID);

				producer.Close();
			}
			else
				throw new EndpointException(Uri, "Unable to create message producer");

			session.Close();
		}

		public IEnumerable<IMessageSelector> SelectiveReceive(TimeSpan timeout)
		{
			Session session = _connection.CreateSession(true, SessionMode.SessionTransacted);
			if (session == null)
				throw new EndpointException(Uri, "Unable to open session to endpoint");

			Destination destination = session.CreateQueue(_queueName);

			MessageConsumer consumer = session.CreateConsumer(destination);
			if (consumer != null)
			{
				Message message;
				do
				{
					message = consumer.Receive((long) timeout.TotalMilliseconds);
					if (message != null)
					{
						using (var selector = new TibcoMessageSelector(this, session, message, _serializer))
						{
							yield return selector;
						}
					}
				} while (message != null);

				consumer.Close();
			}
			else
				throw new EndpointException(Uri, "Unable to create message consumer");

			session.Close();
		}

		public void OnException(EMSException exception)
		{
			_log.Error("An exception occurred on the endpoint. Uri = " + _uri, exception);
		}

		~TibcoEndpoint()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_connection.Stop();
				_connection.Close();
				_connection = null;
			}
			_disposed = true;
		}

		public static IEndpoint ConfigureEndpoint(Uri uri, Action<IEndpointConfigurator> configurator)
		{
			if (uri.Scheme.ToLowerInvariant() == "tibco")
			{
				IEndpoint endpoint = TibcoEndpointConfigurator.New(x =>
					{
						x.SetUri(uri);

						configurator(x);
					});

				return endpoint;
			}

			return null;
		}
	}
}