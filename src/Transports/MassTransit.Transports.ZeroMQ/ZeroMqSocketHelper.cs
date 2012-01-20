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
	using Util;
	using ZMQ;

	public static class ZeroMqSocketHelper
	{
		static Dictionary<SocketType, int> _sockets
			= new Dictionary<SocketType, int>
				{
					{ SocketType.PULL, 0 },
					{ SocketType.PUSH, 1 },
					{ SocketType.SUB, 2 },
					{ SocketType.PUB, 3 },
					{ SocketType.ROUTER, 4 },
					{ SocketType.DEALER, 5 },
					// no other sockets allowed right now.
				};

		public static int OffsetFor(SocketType type)
		{
			if (!_sockets.ContainsKey(type))
				throw new NotSupportedException(string.Format("internal MT error, no support for {0}", type));

			return _sockets[type];
		}

		/// <summary>
		/// Given the base uri (pull socket uri), gives the other socket
		/// uri based on what the other socket type is.
		/// </summary>
		[NotNull]
		public static Uri MakeUriFor([NotNull] this Uri baseUri, SocketType socketType)
		{
			if (baseUri == null) throw new ArgumentNullException("baseUri");
			return NextPortBy(baseUri, OffsetFor(socketType));
		}

		static Uri NextPortBy(Uri uri, int count)
		{
			var uriBuilder = new UriBuilder(uri);
			uriBuilder.Port = uriBuilder.Port + count;
			return uriBuilder.Uri;
		}
	}
}