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
namespace MassTransit.ServiceBus.WCF
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.ServiceModel;
	using System.Threading;
	using log4net;

	public class WcfEndpoint : IEndpoint
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (WcfEndpoint));
		private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Messages");
		private readonly BinaryFormatter _formatter = new BinaryFormatter();

		private readonly Semaphore _messageReady = new Semaphore(0, int.MaxValue);
		private readonly Queue<MessageEnvelope> _messages = new Queue<MessageEnvelope>();
		private readonly Uri _serviceUri;
		private readonly Uri _uri;
		private string _configuration = "MassTransit_EndpointClient";
		private ServiceHost _host;


		public WcfEndpoint(Uri uri)
		{
			_uri = uri;

			UriBuilder builder = new UriBuilder(uri);
			//builder.Scheme = uri.Scheme.Substring(4);

			_serviceUri = builder.Uri;

			_log.DebugFormat("Opening host for WCF endpoint: {0}", _serviceUri);
			_host = new ServiceHost(new InboundMessageHandler(this), _serviceUri);
			_host.Open();
		}

		public static string Scheme
		{
			get { return "net.tcp"; }
		}

		public void Dispose()
		{
			_log.DebugFormat("Closing host for WCF endpoint: {0}", _serviceUri);
			_host.Close();

			lock (_messages)
				_messages.Clear();
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
			if (_messageLog.IsInfoEnabled)
				_messageLog.InfoFormat("SEND:{0}:{1}", Uri, typeof (T).Name);

			MessageEnvelope envelope;

			using (MemoryStream mstream = new MemoryStream())
			{
				_formatter.Serialize(mstream, message);

				envelope = new MessageEnvelope(mstream.ToArray());
			}

			ChannelFactory<IEndpointContract> channelFactory = new ChannelFactory<IEndpointContract>(_configuration);

			channelFactory.Endpoint.Address = new EndpointAddress(_serviceUri);

			IEndpointContract proxy = channelFactory.CreateChannel();
			try
			{
				proxy.Send(envelope);

				((IClientChannel) proxy).Close();
			}
			catch
			{
				((IClientChannel) proxy).Abort();
				throw;
			}
		}

		public object Receive(TimeSpan timeout)
		{
			if (_messageReady.WaitOne(timeout, true))
			{
				try
				{
					object obj = Dequeue();

					if (_messageLog.IsInfoEnabled)
						_messageLog.InfoFormat("RECV:{0}:{1}", _uri, obj.GetType().Name);

					return obj;
				}
				catch (InvalidOperationException ex)
				{
				}
			}

			return null;
		}

		public object Receive(TimeSpan timeout, Predicate<object> accept)
		{
			if (_messageReady.WaitOne(timeout, true))
			{
				try
				{
					object obj = Dequeue();

					if (accept(obj))
					{
						if (_messageLog.IsInfoEnabled)
							_messageLog.InfoFormat("RECV:{0}:{1}", _uri, obj.GetType().Name);

						return obj;
					}
				}
				catch (InvalidOperationException ex)
				{
				}
			}

			return null;
		}

		private void Enqueue(MessageEnvelope message)
		{
			lock (_messages)
				_messages.Enqueue(message);

			_messageReady.Release();
		}

		private object Dequeue()
		{
			MessageEnvelope envelope;
			lock (_messages)
				envelope = _messages.Dequeue();

			using (MemoryStream mstream = new MemoryStream(envelope.Message))
			{
				object obj = _formatter.Deserialize(mstream);

				return obj;
			}
		}

		[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConfigurationName = "MassTransit.DefaultWcfService")]
		public class InboundMessageHandler : IEndpointContract
		{
			private readonly WcfEndpoint _endpoint;

			public InboundMessageHandler(WcfEndpoint endpoint)
			{
				_endpoint = endpoint;
			}

			[OperationBehavior]
			public void Send(MessageEnvelope message)
			{
				_endpoint.Enqueue(message);
			}
		}
	}
}