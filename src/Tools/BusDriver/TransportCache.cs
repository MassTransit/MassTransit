// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace BusDriver
{
	using System;
	using System.Collections.Generic;
	using Magnum.Extensions;
	using MassTransit;
	using MassTransit.Exceptions;
	using MassTransit.Transports;

	public class TransportCache :
		IDisposable
	{
		readonly IDictionary<string, ITransportFactory> _transportFactories;
		readonly IDictionary<string, IInboundTransport> _inboundTransports;
		readonly IDictionary<string, IOutboundTransport> _outboundTransports;
		bool _disposed;

		public TransportCache()
		{
			_transportFactories = new Dictionary<string, ITransportFactory>();
            _inboundTransports = new Dictionary<string, IInboundTransport>();
            _outboundTransports = new Dictionary<string, IOutboundTransport>();
		}

		public void Dispose()
		{
			Dispose(true);
		}

        public IInboundTransport GetInboundTransport(Uri uri)
		{
			string key = uri.ToString().ToLowerInvariant();
            IInboundTransport transport;
            if (_inboundTransports.TryGetValue(key, out transport))
				return transport;

			string scheme = uri.Scheme.ToLowerInvariant();

			ITransportFactory transportFactory;
			if (_transportFactories.TryGetValue(scheme, out transportFactory))
			{
				try
				{
					ITransportSettings settings = new TransportSettings(new EndpointAddress(uri));
					transport = transportFactory.BuildInbound(settings);

                    _inboundTransports.Add(uri.ToString().ToLowerInvariant(), transport);

					return transport;
				}
				catch (Exception ex)
				{
					throw new TransportException(uri, "Failed to create inbound transport", ex);
				}
			}

			throw new TransportException(uri,
				"The {0} scheme was not handled by any registered transport.".FormatWith(uri.Scheme));
		}

        public IOutboundTransport GetOutboundTransport(Uri uri)
		{
			string key = uri.ToString().ToLowerInvariant();
            IOutboundTransport transport;
            if (_outboundTransports.TryGetValue(key, out transport))
				return transport;

			string scheme = uri.Scheme.ToLowerInvariant();

			ITransportFactory transportFactory;
			if (_transportFactories.TryGetValue(scheme, out transportFactory))
			{
				try
				{
					ITransportSettings settings = new TransportSettings(new EndpointAddress(uri));
					transport = transportFactory.BuildOutbound(settings);

                    _outboundTransports.Add(uri.ToString().ToLowerInvariant(), transport);

					return transport;
				}
				catch (Exception ex)
				{
					throw new TransportException(uri, "Failed to create outbound transport", ex);
				}
			}

			throw new TransportException(uri,
				"The {0} scheme was not handled by any registered transport.".FormatWith(uri.Scheme));
		}

		public void AddTransportFactory(ITransportFactory factory)
		{
			string scheme = factory.Scheme.ToLowerInvariant();

			_transportFactories[scheme] = factory;
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_inboundTransports.Values.Each(x => x.Dispose());
				_outboundTransports.Values.Each(x => x.Dispose());
				_transportFactories.Values.Each(x => x.Dispose());
			}

			_disposed = true;
		}
	}
}