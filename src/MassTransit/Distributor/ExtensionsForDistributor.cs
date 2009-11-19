// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Distributor
{
	using System;
	using Configuration;
	using Magnum.DateTimeExtensions;
	using MassTransit.Configuration;
	using Saga;

	public static class ExtensionsForDistributor
	{
		public static void UseDistributorFor<T>(this IServiceBusConfigurator configurator, IEndpointFactory endpointFactory)
			where T : class
		{
			configurator.AddService(() => new Distributor<T>(endpointFactory));

			configurator.SetReceiveTimeout(50.Milliseconds());
		}

		public static void ImplementDistributorWorker<T>(this IServiceBusConfigurator configurator, Func<T, Action<T>> getConsumer)
			where T : class
		{
			configurator.AddService(() => new DistributorWorker<T>(getConsumer));
		}

		public static void UseDistributorForSaga<T>(this IServiceBusConfigurator configurator, IEndpointFactory endpointFactory)
			where T : SagaStateMachine<T>, ISaga
		{
			T saga = (T) Activator.CreateInstance(typeof (T), Guid.NewGuid());

			var serviceConfigurator = new SagaDistributorConfigurator(configurator, endpointFactory);

			saga.EnumerateDataEvents(serviceConfigurator.AddService);
		}
	}
}