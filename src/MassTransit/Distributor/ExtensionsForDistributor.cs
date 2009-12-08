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
	using Magnum;
	using Magnum.Activator;
	using Magnum.DateTimeExtensions;
	using MassTransit.Configuration;
	using Saga;

	public static class ExtensionsForDistributor
	{
        /// <summary>
        /// Implements a distributor-to-worker pattern for the given message type. 
        /// </summary>
        /// <typeparam name="T">The type of message to use the distributor</typeparam>
        /// <param name="configurator">Service bus to implement the distributor</param>
        /// <param name="endpointFactory">Factory to generate endpoints from a given URL</param>
		public static void UseDistributorFor<T>(this IServiceBusConfigurator configurator, IEndpointFactory endpointFactory)
			where T : class
		{
			configurator.AddService(() => new Distributor<T>(endpointFactory));

			configurator.SetReceiveTimeout(50.Milliseconds());
		}

        /// <summary>
        /// Implements a distributor-to-worker pattern for the given message type. 
        /// </summary>
        /// <typeparam name="T">The type of to use the distributor</typeparam>
        /// <typeparam name="K">The <code>IWorkerSelectionStrategy</code> used to pick 
        /// which worker node to send a message</typeparam>
        /// <param name="configurator">Service bus to implement the distributor</param>
        /// <param name="endpointFactory">Factory to generate endpoints from a given URL</param>
        public static void UseDistributorFor<T, K>(this IServiceBusConfigurator configurator, IEndpointFactory endpointFactory)
            where T : class
            where K : class, IWorkerSelectionStrategy<T>, new()
        {
            configurator.AddService(() => new Distributor<T>(endpointFactory, new K()));

            configurator.SetReceiveTimeout(50.Milliseconds());
        }

        /// <summary>
        /// Implements a distributor-to-worker pattern for the given message type. 
        /// </summary>
        /// <typeparam name="T">The type of to use the distributor</typeparam>
        /// <param name="configurator">Service bus to implement the distributor</param>
        /// <param name="endpointFactory">Factory to generate endpoints from a given URL</param>
        /// <param name="workerSelectionStrategy">The <code>IWorkerSelectionStrategy</code> 
        /// used to pick which worker node to send a message</param>
        public static void UseDistributorFor<T>(this IServiceBusConfigurator configurator, IEndpointFactory endpointFactory, 
                                                IWorkerSelectionStrategy<T> workerSelectionStrategy)
            where T : class
        {
            configurator.AddService(() => new Distributor<T>(endpointFactory, workerSelectionStrategy));

            configurator.SetReceiveTimeout(50.Milliseconds());
        }

		public static void ImplementDistributorWorker<T>(this IServiceBusConfigurator configurator, Func<T, Action<T>> getConsumer)
			where T : class
		{
			configurator.AddService(() => new Worker<T>(getConsumer));
		}

		public static void UseSagaDistributorFor<T>(this IServiceBusConfigurator configurator, IEndpointFactory endpointFactory)
			where T : SagaStateMachine<T>, ISaga
		{
			var saga = FastActivator<T>.Create(CombGuid.Generate());

			var serviceConfigurator = new SagaDistributorConfigurator(configurator, endpointFactory);

			saga.EnumerateDataEvents(serviceConfigurator.AddService);
		}

		public static void ImplementSagaDistributorWorker<T>(this IServiceBusConfigurator configurator, ISagaRepository<T> repository)
			where T : SagaStateMachine<T>, ISaga
		{
			configurator.AddService(() => new SagaWorker<T>());
		}
	}
}