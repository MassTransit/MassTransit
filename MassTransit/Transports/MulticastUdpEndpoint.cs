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
namespace MassTransit.Transports
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net;
	using System.Net.Sockets;
	using System.Threading;
	using Configuration;
	using Exceptions;
	using Internal;
	using log4net;
	using Serialization;

	public class MulticastUdpEndpoint :
		IEndpoint
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (MulticastUdpEndpoint));
		private readonly ManualResetEvent _doneReceiving = new ManualResetEvent(false);
		private readonly IMessageSerializer _serializer;
		private readonly ManualResetEvent _shutdown = new ManualResetEvent(false);
		private readonly Uri _uri;
		private volatile bool _disposed;
		private IPAddress _groupAddress;
		private UdpClient _receiveClient;
		private UdpClient _sendClient;
		private IPEndPoint _sendIPEndPoint;

		public MulticastUdpEndpoint(string uriString, IMessageSerializer serializer)
			: this(new Uri(uriString), serializer)
		{
		}

		public MulticastUdpEndpoint(Uri uri, IMessageSerializer serializer)
		{
			_uri = uri;
			_serializer = serializer;

			Initialize();
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

			Type messageType = typeof (T);

			using (MemoryStream mstream = new MemoryStream())
			{
				_serializer.Serialize(mstream, message);

				// if (timeToLive < TimeSpan.MaxValue)

				try
				{
					if (SpecialLoggers.Messages.IsInfoEnabled)
						SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}", Uri, messageType.Name);

					_sendClient.Send(mstream.ToArray(), (int) mstream.Length, _sendIPEndPoint);
				}
				catch (Exception ex)
				{
					throw new EndpointException(this, "Problem sending to " + _sendIPEndPoint, ex);
				}
			}

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Message Sent: Type = {1}", messageType.Name);
		}

		public IEnumerable<IMessageSelector> SelectiveReceive(TimeSpan timeout)
		{
			if (_disposed) throw new ObjectDisposedException("The object has been disposed");

			IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _uri.Port);

			byte[] data = _receiveClient.Receive(ref endPoint);
			if (data != null && data.Length > 0)
				using (MulticastUdpMessageSelector selector = new MulticastUdpMessageSelector(this, data, _serializer))
				{
					yield return selector;
				}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_shutdown.Set();

				if (_sendClient != null)
				{
					using (_sendClient)
						_sendClient.Close();
				}

				if (_receiveClient != null)
				{
					using (_receiveClient)
					{
						_receiveClient.Close();
						_doneReceiving.WaitOne(1000, false);
					}
				}
			}
			_disposed = true;
		}

		private void Initialize()
		{
			_groupAddress = IPAddress.Parse(_uri.Host);
			if (_groupAddress == null)
				throw new ArgumentException("The multicast address could not be determined from the Uri: " + _uri);

			if (_groupAddress.AddressFamily != AddressFamily.InterNetwork)
				throw new ArgumentException("The specified address is not a valid multicast address: " + _uri);

			InitializeReceiver();

			InitializeSender();
		}

		private void InitializeSender()
		{
			_sendIPEndPoint = new IPEndPoint(_groupAddress, _uri.Port);

			const int port = 0;
			_sendClient = new UdpClient(port, AddressFamily.InterNetwork);
			_sendClient.DontFragment = true;
			_sendClient.Ttl = 2; // 0 = host, 1 = subnet, <32 = same company 
			_sendClient.Client.SendBufferSize = 256*1024;

			_sendClient.JoinMulticastGroup(_groupAddress);
		}

		private void InitializeReceiver()
		{
			_receiveClient = new UdpClient();

			Socket s = _receiveClient.Client;

			const int optionValue = 1;
			s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue);

			s.ReceiveBufferSize = 256*1024;
			s.ReceiveTimeout = 2000;
			s.Bind(new IPEndPoint(IPAddress.Any, _uri.Port));

			_receiveClient.JoinMulticastGroup(_groupAddress, 2); // 0 = host, 1 = subnet, <32 = same company
		}

		~MulticastUdpEndpoint()
		{
			Dispose(false);
		}

		public static IEndpoint ConfigureEndpoint(Uri uri, Action<IEndpointConfigurator> configurator)
		{
			if (uri.Scheme.ToLowerInvariant() == "multicast")
			{
				IEndpoint endpoint = MulticastUdpEndpointConfigurator.New(x =>
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