// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.TextFixtures
{
    using System;
    using BusConfigurators;
    using MassTransit.Subscriptions.Coordinator;


    public class ServiceInstance :
        IDisposable
    {
        volatile bool _disposed;
        SubscriptionLoopback _loopback;

        public ServiceInstance(string name, Action<ServiceBusConfigurator> configurator)
        {
            DataBus = ServiceBusFactory.New(x =>
                {
                    x.ReceiveFrom(name);
                    x.SetConcurrentConsumerLimit(5);

                    x.AddSubscriptionObserver((bus, coordinator) =>
                        {
                            _loopback = new SubscriptionLoopback(bus, coordinator);
                            return _loopback;
                        });

                    configurator(x);
                });
        }

        public IServiceBus DataBus { get; private set; }

        public void Dispose()
        {
            Dispose(true);
        }

        public void ConfigureServiceBus(ServiceBusConfigurator configurator)
        {
            configurator.AddSubscriptionObserver((bus, coordinator) =>
                {
                    var loopback = new SubscriptionLoopback(bus, coordinator);
                    loopback.SetTargetCoordinator(_loopback.Router);
                    return loopback;
                });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            DataBus.Dispose();
            DataBus = null;

            _disposed = true;
        }
    }
}