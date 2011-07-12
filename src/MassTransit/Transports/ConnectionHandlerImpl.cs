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

namespace MassTransit.Transports
{
	using System;

	public class ConnectionHandlerImpl<T> :
		ConnectionHandler<T>
		where T : Connection
	{
		readonly T _connection;
		readonly ConnectionPolicyChainImpl _policyChain;
		bool _disposed;

		public ConnectionHandlerImpl(T connection)
		{
			_connection = connection;
			_policyChain = new ConnectionPolicyChainImpl(connection);
			_policyChain.Push(new ConnectOnFirstUsePolicy(connection, _policyChain));
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Use(Action<T> callback)
		{
			_policyChain.Execute(() => callback(_connection));
			_connection.Connect();

			callback(_connection);
		}

		public void ForceReconnect()
		{
			_policyChain.Push(new ReconnectPolicy(_connection, _policyChain, TimeSpan.Zero));
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_connection.Disconnect();
				_connection.Dispose();
			}

			_disposed = true;
		}

		~ConnectionHandlerImpl()
		{
			Dispose(false);
		}
	}
}