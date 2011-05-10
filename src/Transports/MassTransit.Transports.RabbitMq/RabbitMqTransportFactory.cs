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
namespace MassTransit.Transports.RabbitMq
{
	using System;
	using System.Linq;
	using Exceptions;
	using log4net;
	using Magnum.Extensions;
	using Magnum.Threading;
	using RabbitMQ.Client;

	public class RabbitMqTransportFactory :
		ITransportFactory
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (RabbitMqTransportFactory));
		readonly ReaderWriterLockedDictionary<Uri, IConnection> _connectionCache;

		bool _disposed;

		public RabbitMqTransportFactory()
		{
			_connectionCache = new ReaderWriterLockedDictionary<Uri, IConnection>();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public string Scheme
		{
			get { return "rabbitmq"; }
		}

		public IDuplexTransport BuildLoopback(ITransportSettings settings)
		{
			var address = RabbitMqEndpointAddress.Parse(settings.Address.Uri);

			var transport = new LoopbackRabbitMqTransport(address, BuildInbound(settings), BuildOutbound(settings));
			return transport;
		}

		public IInboundTransport BuildInbound(ITransportSettings settings)
		{
			var address = RabbitMqEndpointAddress.Parse(settings.Address.Uri);

			EnsureProtocolIsCorrect(address.Uri);

			return new InboundRabbitMqTransport(address, GetConnection(address));
		}

		public IOutboundTransport BuildOutbound(ITransportSettings settings)
		{
			var address = RabbitMqEndpointAddress.Parse(settings.Address.Uri);

			EnsureProtocolIsCorrect(address.Uri);

			return new OutboundRabbitMqTransport(address, GetConnection(address));
		}

		public IOutboundTransport BuildError(ITransportSettings settings)
		{
			return BuildOutbound(settings);
		}

		public int ConnectionCount()
		{
			return _connectionCache.Count();
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_connectionCache.Values.Each(x =>
					{
						try
						{
							if(x.IsOpen)
								x.Close(200, "disposed");
							x.Dispose();
						}
						catch (Exception ex)
						{
							_log.Warn("Failed to close RabbitMQ connection.", ex);
						}
					});
				_connectionCache.Clear();

				_connectionCache.Dispose();
			}

			_disposed = true;
		}

		IConnection GetConnection(IRabbitMqEndpointAddress address)
		{
			return _connectionCache.Retrieve(address.Uri, () => address.ConnectionFactory.CreateConnection());
		}

		~RabbitMqTransportFactory()
		{
			Dispose(false);
		}


		static void EnsureProtocolIsCorrect(Uri address)
		{
			if (address.Scheme != "rabbitmq")
				throw new EndpointException(address, "Address must start with 'rabbitmq' not '{0}'".FormatWith(address.Scheme));
		}
	}
}