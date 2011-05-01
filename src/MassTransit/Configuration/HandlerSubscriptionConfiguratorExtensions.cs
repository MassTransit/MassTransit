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
namespace MassTransit
{
	using System;
	using SubscriptionBuilders;
	using SubscriptionConfigurators;

	public static class HandlerSubscriptionConfiguratorExtensions
	{
		/// <summary>
		/// Subscribes a message handler (which can be any delegate of the message type,
		/// such as a class instance method, a delegate, or a lambda expression)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="configurator"></param>
		/// <param name="handler"></param>
		/// <returns></returns>
		public static HandlerSubscriptionConfigurator<T> Handler<T>(this SubscriptionBusServiceConfigurator configurator,
		                                                            Action<T> handler)
			where T : class
		{
			var handlerConfigurator = new HandlerSubscriptionConfiguratorImpl<T>(handler);

			var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(handlerConfigurator);

			configurator.AddConfigurator(busServiceConfigurator);

			return handlerConfigurator;
		}
	}
}