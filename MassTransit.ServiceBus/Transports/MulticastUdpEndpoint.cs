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
namespace MassTransit.ServiceBus.Transports
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Net;
	using System.Net.Sockets;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Threading;
	using Exceptions;
	using Internal;
	using log4net;

	public class MulticastUdpEndpoint :
		IEndpoint
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (MulticastUdpEndpoint));
		private readonly ManualResetEvent _doneReceiving = new ManualResetEvent(false);
		private readonly BinaryFormatter _formatter = new BinaryFormatter();
		private readonly Semaphore _messageReady = new Semaphore(0, int.MaxValue);
		private readonly Queue<byte[]> _messages = new Queue<byte[]>();
		private readonly ManualResetEvent _shutdown = new ManualResetEvent(false);
		private readonly Uri _uri;
		private IAsyncResult _asyncResult;
		private IPAddress _groupAddress;
		private UdpClient _receiveClient;
		private UdpClient _sendClient;
		private IPEndPoint _sendIPEndPoint;

		public MulticastUdpEndpoint(Uri uri)
		{
			_uri = uri;

			Initialize();
		}

		public MulticastUdpEndpoint(string uriString)
			: this(new Uri(uriString))
		{
		}

		public static string Scheme
		{
			get { return "multicast"; }
		}

		public void Dispose()
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
			Type messageType = typeof (T);

			using (MemoryStream mstream = new MemoryStream())
			{
				_formatter.Serialize(mstream, message);

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

		public object Receive(TimeSpan timeout)
		{
			if (_messageReady.WaitOne(timeout, true))
			{
				try
				{
					object obj = Dequeue();

					if (SpecialLoggers.Messages.IsInfoEnabled)
						SpecialLoggers.Messages.InfoFormat("RECV:{0}:{1}", _uri, obj.GetType().Name);

					return obj;
				}
				catch (InvalidOperationException)
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
						if (SpecialLoggers.Messages.IsInfoEnabled)
							SpecialLoggers.Messages.InfoFormat("RECV:{0}:{1}", _uri, obj.GetType().Name);

						return obj;
					}
				}
				catch (InvalidOperationException)
				{
				}
			}

			return null;
		}

		private void Initialize()
		{
			_groupAddress = IPAddress.Parse(_uri.Host);
			if (_groupAddress == null)
				throw new ArgumentException("The multicast address could not be determined from the Uri: " + _uri);

			if (_groupAddress.AddressFamily != AddressFamily.InterNetwork)
				throw new ArgumentException("The specified address is not a valid multicast address: " + _uri);

			InitializeSender();

			InitializeReceiver();
		}

		private void InitializeSender()
		{
			_sendIPEndPoint = new IPEndPoint(_groupAddress, _uri.Port);

			_sendClient = new UdpClient(0, AddressFamily.InterNetwork);
			_sendClient.DontFragment = true;
			_sendClient.Ttl = 2; // 0 = host, 1 = subnet, <32 = same company

			_sendClient.JoinMulticastGroup(_groupAddress);
		}

		private void InitializeReceiver()
		{
			_receiveClient = new UdpClient();

			Socket s = _receiveClient.Client;

			s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
			s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 100000);

			s.Bind(new IPEndPoint(IPAddress.Any, _uri.Port));

			_receiveClient.JoinMulticastGroup(_groupAddress, 2);

			// kick off the initial receive
			_asyncResult = _receiveClient.BeginReceive(ReceiveComplete, this);
		}

		private void ReceiveComplete(IAsyncResult ar)
		{
			IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _uri.Port);

			try
			{
				byte[] data = _receiveClient.EndReceive(_asyncResult, ref endPoint);

				if (data.Length > 0)
				{
					lock (_messages)
						_messages.Enqueue(data);

					_messageReady.Release();
				}

				_asyncResult = _receiveClient.BeginReceive(ReceiveComplete, this);
			}
			catch (SocketException ex)
			{
				if (ex.SocketErrorCode == SocketError.Shutdown)
					_doneReceiving.Set();
			}
			catch (ObjectDisposedException ex)
			{
			}
		}

		private object Dequeue()
		{
			byte[] buffer;
			lock (_messages)
				buffer = _messages.Dequeue();

			using (MemoryStream mstream = new MemoryStream(buffer))
			{
				object obj = _formatter.Deserialize(mstream);

				return obj;
			}
		}
	}
}