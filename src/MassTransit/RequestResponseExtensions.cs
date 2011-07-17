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
	using RequestResponse;
	using RequestResponse.Configurators;

	public static class RequestResponseExtensions
	{
		public static bool PublishRequest<TRequest>(this IServiceBus bus, TRequest message,
		                                                                Action<RequestConfigurator<TRequest, Guid>> configureCallback)
			where TRequest : class, CorrelatedBy<Guid>
		{
			return PublishRequest<TRequest, Guid>(bus, message, configureCallback);
		}

		public static bool PublishRequest<TRequest, TKey>(this IServiceBus bus, TRequest message,
		                                                                      Action<RequestConfigurator<TRequest, TKey>> configureCallback)
			where TRequest : class, CorrelatedBy<TKey>
		{
			IRequest<TRequest, TKey> request = RequestConfiguratorImpl<TRequest,TKey>.Create(bus, message, configureCallback);

			bus.Publish(message, context => context.SendResponseTo(bus));

			return request.Wait();
		}
	}
}