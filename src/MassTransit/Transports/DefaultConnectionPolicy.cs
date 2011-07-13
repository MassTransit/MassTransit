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
	using Magnum.Extensions;

	public class DefaultConnectionPolicy :
		ConnectionPolicy
	{
		readonly ConnectionHandler _connectionHandler;
		TimeSpan _reconnectDelay;

		public DefaultConnectionPolicy(ConnectionHandler connectionHandler)
		{
			_connectionHandler = connectionHandler;
			_reconnectDelay = 1.Seconds();
		}

		public void Execute(Action callback)
		{
			try
			{
				callback();
			}
			catch (InvalidConnectionException ex)
			{
				_connectionHandler.ForceReconnect(_reconnectDelay);

				throw ex.InnerException;
			}
		}
	}
}