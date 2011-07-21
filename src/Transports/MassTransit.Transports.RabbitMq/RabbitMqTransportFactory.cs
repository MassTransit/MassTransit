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
	using Magnum.Extensions;
	using Magnum.Threading;
	using Management;
	using RabbitMQ.Client;
	using log4net;

	public class RabbitMqTransportFactory :
		ITransportFactory
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (RabbitMqTransportFactory));
		readonly ReaderWriterLockedDictionary<Uri, ConnectionHandler<RabbitMqConnection>> _connectionCache;

		bool _disposed;

		public RabbitMqTransportFactory()
		{
			_connectionCache = new ReaderWriterLockedDictionary<Uri, ConnectionHandler<RabbitMqConnection>>();
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
			RabbitMqEndpointAddress address = RabbitMqEndpointAddress.Parse(settings.Address.Uri);

			var transport = new Transport(address, () => BuildInbound(settings), () => BuildOutbound(settings));

			return transport;
		}

		public IInboundTransport BuildInbound(ITransportSettings settings)
		{
			RabbitMqEndpointAddress address = RabbitMqEndpointAddress.Parse(settings.Address.Uri);

			EnsureProtocolIsCorrect(address.Uri);

			ConnectionHandler<RabbitMqConnection> connectionHandler = GetConnection(address);

			if (settings.PurgeExistingMessages)
			{
				PurgeExistingMessages(connectionHandler, address);
			}

			return new InboundRabbitMqTransport(address, connectionHandler);
		}

		public IOutboundTransport BuildOutbound(ITransportSettings settings)
		{
			RabbitMqEndpointAddress address = RabbitMqEndpointAddress.Parse(settings.Address.Uri);

			EnsureProtocolIsCorrect(address.Uri);

			ConnectionHandler<RabbitMqConnection> connectionHandler = GetConnection(address);

			return new OutboundRabbitMqTransport(address, connectionHandler);
		}

		public IOutboundTransport BuildError(ITransportSettings settings)
		{
			RabbitMqEndpointAddress address = RabbitMqEndpointAddress.Parse(settings.Address.Uri);

			EnsureProtocolIsCorrect(address.Uri);

			ConnectionHandler<RabbitMqConnection> connection = GetConnection(address);

			BindErrorExchangeToQueue(address, connection);

			return new OutboundRabbitMqTransport(address, connection);
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
				_connectionCache.Values.Each(x => x.Dispose());
				_connectionCache.Clear();

				_connectionCache.Dispose();
			}

			_disposed = true;
		}

		ConnectionHandler<RabbitMqConnection> GetConnection(IRabbitMqEndpointAddress address)
		{
			return _connectionCache.Retrieve(address.Uri, () =>
				{
					var connection = new RabbitMqConnection(address);
					var connectionHandler = new ConnectionHandlerImpl<RabbitMqConnection>(connection);
					return connectionHandler;
				});
		}

		~RabbitMqTransportFactory()
		{
			Dispose(false);
		}

		static void PurgeExistingMessages(ConnectionHandler<RabbitMqConnection> connectionHandler,
		                                  IRabbitMqEndpointAddress address)
		{
			connectionHandler.Use(connection =>
				{
					using (var management = new RabbitMqEndpointManagement(address, connection.Connection))
					{
						management.Purge(address.Name);
					}
				});
		}


		static void BindErrorExchangeToQueue(IRabbitMqEndpointAddress address,
		                                     ConnectionHandler<RabbitMqConnection> connectionHandler)
		{
			// we need to go ahead and bind a durable queue for the error transport, since
			// there is probably not a listener for it.

			connectionHandler.Use(connection =>
				{
					using (var management = new RabbitMqEndpointManagement(address, connection.Connection))
					{
						management.BindQueue(address.Name, address.Name, ExchangeType.Fanout, "");
					}
				});
		}


		static void EnsureProtocolIsCorrect(Uri address)
		{
			if (address.Scheme != "rabbitmq")
				throw new EndpointException(address, "Address must start with 'rabbitmq' not '{0}'".FormatWith(address.Scheme));
		}
	}
}