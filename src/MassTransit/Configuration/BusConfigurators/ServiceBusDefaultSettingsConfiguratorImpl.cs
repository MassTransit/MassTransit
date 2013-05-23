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
namespace MassTransit.BusConfigurators
{
    using System;


    public class ServiceBusDefaultSettingsConfiguratorImpl :
        ServiceBusDefaultSettingsConfigurator
    {
        readonly ServiceBusDefaultSettings _defaultSettings;

        public ServiceBusDefaultSettingsConfiguratorImpl(ServiceBusDefaultSettings defaultSettings)
        {
            _defaultSettings = defaultSettings;
        }

        public void SetEndpointCache(IEndpointCache endpointCache)
        {
            _defaultSettings.EndpointCache = endpointCache;
        }

        public void DisableAutoStart()
        {
            _defaultSettings.AutoStart = false;
        }

        public void SetReceiveTimeout(TimeSpan receiveTimeout)
        {
            _defaultSettings.ReceiveTimeout = receiveTimeout;
        }

        public void SetConcurrentConsumerLimit(int concurrentConsumerLimit)
        {
            _defaultSettings.ConcurrentConsumerLimit = concurrentConsumerLimit;
        }

        public void SetConcurrentReceiverLimit(int concurrentReceiverLimit)
        {
            _defaultSettings.ConcurrentReceiverLimit = concurrentReceiverLimit;
        }

        public void EnableAutoStart()
        {
            _defaultSettings.AutoStart = true;
        }

        public void DisablePerformanceCounters()
        {
            _defaultSettings.EnablePerformanceCounters = false;
        }

        public void EnablePerformanceCounters()
        {
            _defaultSettings.EnablePerformanceCounters = true;
        }
    }
}