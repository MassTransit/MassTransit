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
	using System.Diagnostics;
	using Util;
	using ZMQ;
	using log4net;
	using H = ZeroMqSocketHelper;

	[DebuggerDisplay("Connected:{_connected}")]
	public class ZeroMqConnection :
		Connection
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (ZeroMqConnection));

		readonly Context _context;
		readonly SocketType _socketType;
		
		Lazy<Socket>[] _sockets;
		readonly ZeroMqAddress _address;

		[UsedImplicitly]
		bool _connected;

		public ZeroMqConnection(Context context,
		                        ZeroMqAddress address,
		                        SocketType socketType)
		{
			_context = context;
			_address = address;
			_socketType = socketType;
		}

		public void Dispose()
		{
			Disconnect();
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
			get { return ; }
		}

		/// <summary>
		/// Outgoing signals
		/// </summary>
		public Uri PushSocket
		{
			get { return NextPortBy(1); }
		}

		/// <summary>
		/// Incoming data by subscription.
		/// </summary>
		public Uri SubSocket
		{
			get { return NextPortBy(2); }
		}

		/// <summary>
		/// Outgoing data per subscription.
		/// </summary>
		public Uri PubSocket
		{
			get { return NextPortBy(3); }
		}

		/// <summary>
		/// Incoming socket for routing.
		/// </summary>
		public Uri RouterSocket
		{
			get { return NextPortBy(4); }
		}
		/// <summary>
		/// Outgoing fair-routing socket.
		/// </summary>
		public Uri DealerSocket
		{
			get { return NextPortBy(5); }
		}

		public void Connect()
		{
			Disconnect();

			_sockets =  _context.Socket(_socketType);
			//this needs to be configurable maybe the '/queue' part of the uri?
			

			var addr = _address.PullSocket.ToString();
			_sockets.Connect(addr);

			_connected = true;
		}

		public void Disconnect()
		{
			try
			{
				if (_sockets != null)
				{
					_sockets.Dispose();
				}
				_sockets = null;
			}
			catch (System.Exception ex)
			{
				_log.Warn("Failed to close ZeroMq connection.", ex);
				throw;
			}
			_connected = false;
		}
	}
}