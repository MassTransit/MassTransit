// Copyright 2007-2011 Henrik Feldt
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

	[DebuggerDisplay("ZMQ: {RebuiltUri}")]
	public class ZeroMqAddress : IZeroMqEndpointAddress
	{
		readonly ZeroMqAddressComponents _components;

		public ZeroMqAddress([NotNull] Uri uri)
		{
			if (uri == null) throw new ArgumentNullException("uri");

			ZeroMqAddressComponents components;
			IEnumerable<string> reasons;

			if (!TryParse(uri, out components, out reasons))
				BadZeroMqAddress(reasons);
			
			_components = components;
		}

		private ZeroMqAddress([NotNull] ZeroMqAddressComponents components)
		{
			if (components == null) throw new ArgumentNullException("components");

			_components = components;
		}

		static Transport GetTransport(Uri uri)
		{
			var substring = uri.Scheme.Replace("zmq-", "");
			return (Transport) Enum.Parse(typeof (Transport), substring, true);
		}

		public string Host { get; private set; }

		public int Port { get; private set; }

		public Transport ZmqTransport { get; private set; }

		/// <summary>
		/// Incoming signals
		/// </summary>
		public Uri PullSocket
		{
			get { return _components.RebuiltUri; }
		}

		/// <summary>
		/// Outgoing signals
		/// </summary>
		public Uri PushSocket
		{
			get { return _components.RebuiltUri.MakeUriFor(SocketType.PUSH); }
		}

		/// <summary>
		/// Incoming data by subscription.
		/// </summary>
		public Uri SubSocket
		{
			get { return _components.RebuiltUri.MakeUriFor(SocketType.SUB); }
		}

		/// <summary>
		/// Outgoing data per subscription.
		/// </summary>
		public Uri PubSocket
		{
			get { return _components.RebuiltUri.MakeUriFor(SocketType.PUB); }
		}

		/// <summary>
		/// Incoming socket for routing.
		/// </summary>
		public Uri RouterSocket
		{
			get { return _components.RebuiltUri.MakeUriFor(SocketType.ROUTER); }
		}
		/// <summary>
		/// Outgoing fair-routing socket.
		/// </summary>
		public Uri DealerSocket
		{
			get { return _components.RebuiltUri.MakeUriFor(SocketType.DEALER); }
		}

		/// <summary>
		/// Gets the base URI where the pull socket is, that is the base URI for
		/// this bus.
		/// </summary>
		public Uri Uri
		{
			get { return PullSocket; }
		}

		/// <summary>Always the case for zmq.</summary>
		public bool IsLocal { get { return true; } }

		/// <summary>Never the case for zmq.</summary>
		public bool IsTransactional { get { return false; } }

		class ZeroMqAddressComponents : IEquatable<ZeroMqAddressComponents>
		{
			public Uri RawUri { get; set; }
			public Uri RebuiltUri { get; set; }

			public Transport Transport { get; set; }
			public string Host { get; set; }

			public ushort StartPort { get; set; }

			public bool Equals(ZeroMqAddressComponents other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return other.StartPort == StartPort && Equals(other.Host, Host) && Equals(other.Transport, Transport) && Equals(other.RebuiltUri, RebuiltUri) && Equals(other.RawUri, RawUri);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != typeof (ZeroMqAddressComponents)) return false;
				return Equals((ZeroMqAddressComponents) obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					int result = StartPort.GetHashCode();
					result = (result*397) ^ Host.GetHashCode();
					result = (result*397) ^ Transport.GetHashCode();
					result = (result*397) ^ RebuiltUri.GetHashCode();
					result = (result*397) ^ RawUri.GetHashCode();
					return result;
				}
			}

			public static bool operator ==(ZeroMqAddressComponents left, ZeroMqAddressComponents right)
			{
				return Equals(left, right);
			}

			public static bool operator !=(ZeroMqAddressComponents left, ZeroMqAddressComponents right)
			{
				return !Equals(left, right);
			}
		}

		/// <returns>components and reasons are mutually exclusively null.</returns>
		private static bool TryParse(Uri uri, out ZeroMqAddressComponents components, out IEnumerable<string> reasons)
		{
			if (uri.ToString().EndsWith("/")) // initially illegal
			{
				components = null;
				reasons = new[] {"The uri must not end with the '/' character."};
				return false;
			}

			var zmqTransport = GetTransport(uri);
			reasons = null;
			components = new ZeroMqAddressComponents
				{
					RawUri = uri,
					RebuiltUri = new UriBuilder(zmqTransport.ToString(),uri.Host, uri.Port).Uri,
					Host = uri.Host,
					StartPort = checked((ushort)uri.Port), // otherwise, throws now instead of later
					Transport = zmqTransport
				};
			return true;
		}

		public static bool TryParse(Uri uri, out ZeroMqAddress address, out IEnumerable<string> reasons)
		{
			ZeroMqAddressComponents components;

			if (!TryParse(uri, out components, out reasons))
			{
				address = null;
				return false;
			}

			address = new ZeroMqAddress(components);
			return true;
		}

		public static ZeroMqAddress Parse(Uri uri)
		{
			ZeroMqAddress address;

			IEnumerable<string> reasons;
			if (!TryParse(uri, out address, out reasons))
				BadZeroMqAddress(reasons);

			return address;
		}

		static void BadZeroMqAddress(IEnumerable<string> reasons)
		{
			throw new FormatException(string.Format("Unable to parse zmq address: {0}",
				string.Join(". ", reasons)));
		}
	}
} 