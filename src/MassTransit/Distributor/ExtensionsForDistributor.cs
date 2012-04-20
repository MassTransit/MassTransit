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
namespace MassTransit.Distributor
{
    using System;
    using Advanced;
    using BusConfigurators;
    using Configuration;
    using DistributorConfigurators;
    using Magnum.Extensions;
    using Magnum.Reflection;
    using Saga;

    public static class ExtensionsForDistributor
    {
        /// <summary>
        /// Implements a distributor-to-worker pattern for the given message type. 
        /// </summary>
        /// <typeparam name="T">The type of message to use the distributor</typeparam>
        /// <param name="configurator">Service bus to implement the distributor</param>
        public static void UseDistributorFor<T>(this ServiceBusConfigurator configurator)
            where T : class
        {
            //configurator.AddService(BusServiceLayer.Presentation, () => new Distributor<T>());

            configurator.SetReceiveTimeout(50.Milliseconds());
        }

        /// <summary>
        /// Implements a distributor-to-worker pattern for the given message type. 
        /// </summary>
        /// <typeparam name="TMessage">The type of to use the distributor</typeparam>
        /// <typeparam name="TSelectionStrategy">The <code>IWorkerSelectionStrategy</code> used to pick 
        /// which worker node to send a message</typeparam>
        /// <param name="configurator">Service bus to implement the distributor</param>
        public static void UseDistributorFor<TMessage, TSelectionStrategy>(this ServiceBusConfigurator configurator)
            where TMessage : class
            where TSelectionStrategy : class, IWorkerSelector<TMessage>, new()
        {
            //configurator.AddService(BusServiceLayer.Presentation,
           //     () => new Distributor<TMessage>(new TSelectionStrategy()));

            configurator.SetReceiveTimeout(50.Milliseconds());
        }

        /// <summary>
        /// Implements a distributor-to-worker pattern for the given message type. 
        /// </summary>
        /// <typeparam name="TMessage">The type of to use the distributor</typeparam>
        /// <param name="configurator">Service bus to implement the distributor</param>
        /// <param name="workerSelectionStrategy">The <code>IWorkerSelectionStrategy</code> 
        /// used to pick which worker node to send a message</param>
        public static void UseDistributorFor<TMessage>(this ServiceBusConfigurator configurator,
                                                       IWorkerSelector<TMessage> workerSelectionStrategy)
            where TMessage : class
        {
           // configurator.AddService(BusServiceLayer.Presentation,
           //     () => new Distributor<TMessage>(workerSelectionStrategy));

            configurator.SetReceiveTimeout(50.Milliseconds());
        }

        public static void ImplementDistributorWorker<TMessage>(this ServiceBusConfigurator configurator,
                                                                Func<TMessage, Action<TMessage>> getConsumer)
            where TMessage : class
        {
           // configurator.AddService(BusServiceLayer.Presentation, () => new Worker<TMessage>(getConsumer));
        }

        public static void ImplementDistributorWorker<TMessage>(this ServiceBusConfigurator configurator,
                                                                Func<TMessage, Action<TMessage>> getConsumer,
                                                                int inProgressLimit, int pendingLimit)
            where TMessage : class
        {
            var settings = new WorkerSettings
                {
                    InProgressLimit = inProgressLimit,
                    PendingLimit = pendingLimit
                };

           // configurator.AddService(BusServiceLayer.Presentation, () => new Worker<TMessage>(getConsumer, settings));
        }

        public static void UseSagaDistributorFor<T>(this ServiceBusConfigurator configurator)
            where T : SagaStateMachine<T>, ISaga
        {
            T saga = FastActivator<T>.Create(NewId.NextGuid());

//            var serviceConfigurator = new SagaDistributorConfigurator(configurator);

//            saga.EnumerateDataEvents(serviceConfigurator.AddService);
        }

        public static void ImplementSagaDistributorWorker<T>(this ServiceBusConfigurator configurator,
                                                             ISagaRepository<T> repository)
            where T : SagaStateMachine<T>, ISaga
        {
            configurator.AddService(BusServiceLayer.Presentation, bus => new SagaWorker<T>(repository));
        }
    }
}