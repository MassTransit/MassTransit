// Copyright 2007-2011 Dru Sellers, Henrik Feldt
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
namespace MassTransit.Transports.ZeroMq
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using Util;
	using ZMQ;
	using log4net;
	using H = ZeroMqSocketHelper;
	using ST = ZMQ.SocketType;
	using System.Linq;

	[DebuggerDisplay("Connected:{_connected}")]
	public class ZeroMqConnection :
		Connection
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (ZeroMqConnection));

		readonly Context _context;

		List<Lazy<Socket>> _sockets;
		readonly ZeroMqAddress _address;

		[UsedImplicitly]
		bool _connected;
		bool _disposed;

		public ZeroMqConnection(Context context,
		                        ZeroMqAddress address)
		{
			_context = context;
			_address = address;
		}

		Lazy<Socket> SocketFor(SocketType type)
		{
			return _sockets[H.OffsetFor(type)];
		}

		/// <summary>
		/// Incoming signals
		/// </summary>
		public Socket PullSocket
		{
			get { return SocketFor(ST.PULL).Value ; }
		}

		/// <summary>
		/// Outgoing signals
		/// </summary>
		public Socket PushSocket
		{
			get { return SocketFor(ST.PUSH).Value; }
		}

		/// <summary>
		/// Incoming data by subscription.
		/// </summary>
		public Socket SubSocket
		{
			get { return SocketFor(ST.SUB).Value; }
		}

		/// <summary>
		/// Outgoing data per subscription.
		/// </summary>
		public Socket PubSocket
		{
			get { return SocketFor(ST.PUB).Value; }
		}

		/// <summary>
		/// Incoming socket for routing.
		/// </summary>
		public Socket RouterSocket
		{
			get { return SocketFor(ST.ROUTER).Value; }
		}

		/// <summary>
		/// Outgoing fair-routing socket.
		/// </summary>
		public Socket DealerSocket
		{
			get { return SocketFor(ST.DEALER).Value; }
		}

		public Context Context
		{
			get { return _context; }
		}

		/// <summary>
		/// Gets the address
		/// </summary>
		public ZeroMqAddress Address { get { return _address; } }

		public void Connect()
		{
			// prepare the sockets
			foreach (var st in new[] {ST.PUSH, ST.PULL, ST.PUB, ST.SUB, ST.ROUTER, ST.DEALER})
			{
				var stt = st;
				_sockets[H.OffsetFor(stt)] = new Lazy<Socket>(() => _context.Socket(stt));
			}
			_connected = true;
		}

		public void Dispose()
		{
			if (_disposed)
				throw new ObjectDisposedException("already disposed");
			
			_disposed = false;
			
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool managed)
		{
			if (managed)
				Disconnect();
		}

		public void Disconnect()
		{
			if (!_connected)
				return;

			try
			{
				_sockets
					.Where(x => x.IsValueCreated)
					.ToList()
					.ForEach(socket => socket.Value.Dispose());
			}
			finally
			{
				_connected = false;
			}
		}
	}
}