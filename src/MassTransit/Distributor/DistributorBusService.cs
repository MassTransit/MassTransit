// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using DistributorConnectors;
    using Magnum.Caching;
    using Magnum.Extensions;
    using Subscriptions;

    public class DistributorBusService :
        IDistributor,
        IBusService
    {
        readonly IList<DistributorConnector> _connectors;
        readonly IList<ISubscriptionReference> _subscriptions;
        readonly Cache<Type, IWorkerAvailability> _workerAvailabilityCache;
        readonly IWorkerCache _workerCache;

        IServiceBus _bus;
        IServiceBus _controlBus;
        bool _disposed;

        public DistributorBusService(IList<DistributorConnector> connectors)
        {
            _connectors = connectors;

            _subscriptions = new List<ISubscriptionReference>();
            _workerAvailabilityCache = new ConcurrentCache<Type, IWorkerAvailability>();
            _workerCache = new DistributorWorkerCache();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Start(IServiceBus bus)
        {
            _bus = bus;
            _controlBus = bus.ControlBus;

            bus.Configure(pipelineConfigurator =>
                {
                    foreach (DistributorConnector connector in _connectors)
                    {
                        try
                        {
                            ISubscriptionReference subscription = connector.Connect(pipelineConfigurator, this);
                            _subscriptions.Add(subscription);
                        }
                        catch (Exception)
                        {
                            StopAllSubscriptions();
                            throw;
                        }
                    }

                    return () => true;
                });
        }

        public void Stop()
        {
            StopAllSubscriptions();
        }

        public IWorkerAvailability<TMessage> GetWorkerAvailability<TMessage>()
            where TMessage : class
        {
            IWorkerAvailability workerAvailability = _workerAvailabilityCache.Get(typeof(TMessage),
                _ => AddMessageWorkerAvailability<TMessage>());

            return workerAvailability as IWorkerAvailability<TMessage>;
        }

        IWorkerAvailability AddMessageWorkerAvailability<TMessage>()
            where TMessage : class
        {
            var workerAvailability = new MessageWorkerAvailability<TMessage>(_workerCache);

            UnsubscribeAction unsubscribeAction = _controlBus.SubscribeInstance(workerAvailability);

            _subscriptions.Add(new TransientSubscriptionReference(unsubscribeAction));

            return workerAvailability;
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _connectors.Clear();
                _subscriptions.Clear();
            }

            _disposed = true;
        }

        void StopAllSubscriptions()
        {
            _subscriptions.Each(x => x.OnStop());
            _subscriptions.Clear();
        }
    }
}