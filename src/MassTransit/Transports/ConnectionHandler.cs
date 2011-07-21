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

	public interface ConnectionHandler
	{
		void Connect();
		void Disconnect();
		void ForceReconnect(TimeSpan reconnectDelay);
	}

	/// <summary>
	/// Wraps the management of a connection to apply reconnect and retry strategies
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ConnectionHandler<T> :
		IDisposable
		where T : Connection
	{
		void Use(Action<T> callback);

		void AddBinding(ConnectionBinding<T> binding);
		void RemoveBinding(ConnectionBinding<T> binding);
	}
}