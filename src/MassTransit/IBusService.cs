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

using MassTransit.Util;

namespace MassTransit
{
	using System;

	/// <summary>
	/// <para>A bus service is an extension to the service bus, and is used to create services that
	/// add functionality to the service bus, such as subscription managers, distributors, etc.</para>
	/// 
	/// <para>This interface should not be used in most situations and is typically an internal
	/// use thing.</para>
	/// 
	/// <para>Have a look at <see cref="BusServiceLayer"/> for the different levels the service can live in.</para>
	/// </summary>
	public interface IBusService :
		IDisposable
	{
		/// <summary>
		/// Called when the service is being started, which is after the service bus has been started.
		/// </summary>
		/// <param name="bus">The service bus</param>
		void Start( IServiceBus bus);

		/// <summary>
		/// Called when the ServiceBus is being disposed, to allow any resources or subscriptions
		/// to be released.
		/// </summary>
		void Stop();
	}
}