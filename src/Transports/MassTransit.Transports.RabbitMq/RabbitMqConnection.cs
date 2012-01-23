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
	using Magnum.Extensions;
	using RabbitMQ.Client;
	using log4net;

	public class RabbitMqConnection :
		Connection
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (RabbitMqConnection));
		bool _disposed;
		IConnection _connection;
		readonly ConnectionFactory _connectionFactory;

		public RabbitMqConnection(ConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public IConnection Connection
		{
			get { return _connection; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool managed)
		{
			if (!managed)
				return;

			if (_disposed)
				throw new ObjectDisposedException("RabbitMqConnection for {0}".FormatWith(_connectionFactory.Address), "Cannot dispose a connection twice");

			try
			{
				Disconnect();
			}
			finally
			{
				_disposed = true;
				
			}
		}

		public void Connect()
		{
			Disconnect();

			_connection = _connectionFactory.CreateConnection();
		}

		public void Disconnect()
		{
			try
			{
				if (_connection != null)
				{
					if (_connection.IsOpen)
						_connection.Close(200, "disconnected");

					_connection.Dispose();
					_connection = null;
				}
			}
			catch (Exception ex)
			{
				_log.Warn("Failed to close RabbitMQ connection.", ex);
			}
		}
	}
}