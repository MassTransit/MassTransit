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
	using System.Diagnostics;
	using System.IO;
	using System.Threading;
	using Configuration;
	using Internal;
	using log4net;
	using Serialization;

    [DebuggerDisplay("{Uri}")]
	public class LoopbackEndpoint :
		IEndpoint
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (LoopbackEndpoint));
		private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Messages");

		private readonly AutoResetEvent _messageReady = new AutoResetEvent(false);

		public readonly LinkedList<byte[]> _messages = new LinkedList<byte[]>();

		private readonly IMessageSerializer _serializer;
		private readonly Uri _uri;
		private bool _disposed;

		public LoopbackEndpoint(Uri uri)
		{
			_uri = uri;
			_serializer = new XmlMessageSerializer();
		}

		public LoopbackEndpoint(Uri uri, IMessageSerializer serializer)
		{
			_uri = uri;
			_serializer = serializer;
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

			if (_messageLog.IsInfoEnabled)
				_messageLog.InfoFormat("SEND:{0}:{1}", Uri, typeof (T).Name);

			OutboundMessage.Set(headers =>
				{
					headers.SetMessageType(typeof (T));
					headers.SetDestinationAddress(Uri);
				});

			Enqueue(message);
		}

		public IEnumerable<IMessageSelector> SelectiveReceive(TimeSpan timeout)
		{
			if (_disposed) throw new ObjectDisposedException("The object has been disposed");

			if (_messages.Count == 0)
			{
				if (!_messageReady.WaitOne(timeout, true))
					yield break;
			}

			var iterator = _messages.First;
			while (iterator != null)
			{
				var node = iterator;
				Action acceptor = () => _messages.Remove(node);

				using (var selector = new LoopbackMessageSelector(this, node.Value, acceptor, _serializer))
				{
					yield return selector;

					if (selector.AcceptedMessage)
						yield break;
				}

				iterator = iterator.Next;
			}

			_messageReady.WaitOne(timeout, true);
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
				lock (_messages)
					_messages.Clear();
			}
			_disposed = true;
		}

		private void Enqueue<T>(T message)
		{
			using (MemoryStream mstream = new MemoryStream())
			{
				_serializer.Serialize(mstream, message);

				_messages.AddLast(mstream.ToArray());
			}

			_messageReady.Set();
		}

		public static IEndpoint ConfigureEndpoint(Uri uri, Action<IEndpointConfigurator> configurator)
		{
			if (uri.Scheme.ToLowerInvariant() == "loopback")
			{
				IEndpoint endpoint = LoopbackEndpointConfigurator.New(x =>
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