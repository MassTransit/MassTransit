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
    using Magnum.Caching;
    using Magnum.Extensions;
    using Stact;
    using Stact.Executors;
    using Stact.Internal;
    using Subscriptions;
    using WorkerConnectors;

    public class WorkerBusService :
        IWorker,
        IBusService
    {
        readonly IList<WorkerConnector> _connectors;
        readonly IList<ISubscriptionReference> _subscriptions;
        readonly Cache<Type, IWorkerLoad> _workerLoadCache;
        readonly Fiber _fiber;
        readonly Scheduler _scheduler;

        IServiceBus _bus;
        IServiceBus _controlBus;
        bool _disposed;
        TimeSpan _publishInterval = 5.Seconds();

        public WorkerBusService(IList<WorkerConnector> connectors)
        {
            _connectors = connectors;

            _subscriptions = new List<ISubscriptionReference>();
            _workerLoadCache = new GenericTypeCache<IWorkerLoad>(typeof(IWorkerLoad<>));

            _fiber = new PoolFiber(new TryCatchOperationExecutor());
            _scheduler = new TimerScheduler(new PoolFiber(new TryCatchOperationExecutor()));
        }

        public IServiceBus ControlBus
        {
            get { return _controlBus; }
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
                    foreach (WorkerConnector connector in _connectors)
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

            _scheduler.Schedule(TimeSpan.Zero, _publishInterval, _fiber, PublishWorkerAvailability);
        }

        void PublishWorkerAvailability()
        {
            _workerLoadCache.Each(x =>
                {
                    x.PublishWorkerAvailability(_publishInterval);
                });
        }

        public void Stop()
        {
            StopAllSubscriptions();
        }

        public IServiceBus Bus
        {
            get { return _bus; }
        }

        public Uri ControlUri
        {
            get { return _controlBus.Endpoint.Address.Uri; }
        }

        public Uri DataUri
        {
            get { return _bus.Endpoint.Address.Uri; }
        }

        public IWorkerLoad<TMessage> GetWorkerLoad<TMessage>()
            where TMessage : class
        {
            IWorkerLoad workerLoad = _workerLoadCache.Get(typeof(TMessage),
                _ => AddWorkerLoad<TMessage>());

            return workerLoad as IWorkerLoad<TMessage>;
        }

        IWorkerLoad AddWorkerLoad<TMessage>()
            where TMessage : class
        {
            var workerLoad = new MessageWorkerLoad<TMessage>(this);

            UnsubscribeAction unsubscribeAction = _controlBus.SubscribeInstance(workerLoad);

            _subscriptions.Add(new TransientSubscriptionReference(unsubscribeAction));

            return workerLoad;
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