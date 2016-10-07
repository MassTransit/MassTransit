// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;


    public class DeliveryTracker :
        IDeliveryTracker
    {
        readonly ZeroActiveDeliveryHandler _handler;
        int _activeDeliveryCount;
        long _deliveryCount;
        int _maxConcurrentDeliveryCount;

        public DeliveryTracker(ZeroActiveDeliveryHandler handler)
        {
            _handler = handler;
        }

        public DeliveryTracker()
        {
            _handler = () =>
            {
            };
        }

        public int ActiveDeliveryCount => _activeDeliveryCount;
        public long DeliveryCount => _deliveryCount;
        public int MaxConcurrentDeliveryCount => _maxConcurrentDeliveryCount;

        public DeliveryMetrics GetDeliveryMetrics()
        {
            return new Metrics(_deliveryCount, _maxConcurrentDeliveryCount);
        }

        public IDelivery BeginDelivery()
        {
            var current = Interlocked.Increment(ref _activeDeliveryCount);
            while (current > _maxConcurrentDeliveryCount)
                Interlocked.CompareExchange(ref _maxConcurrentDeliveryCount, current, _maxConcurrentDeliveryCount);

            return new Delivery(Interlocked.Increment(ref _deliveryCount), DeliveryComplete);
        }

        public void DeliveryComplete(long id)
        {
            var pendingCount = Interlocked.Decrement(ref _activeDeliveryCount);
            if (pendingCount == 0)
            {
                _handler();
            }
        }


        struct Delivery :
            IDelivery
        {
            readonly long _id;
            readonly Action<long> _complete;

            long IDelivery.Id => _id;

            public Delivery(long id, Action<long> complete)
            {
                _id = id;
                _complete = complete;
            }

            public void Dispose()
            {
                _complete(_id);
            }
        }


        class Metrics :
            DeliveryMetrics
        {
            public Metrics(long deliveryCount, int concurrentDeliveryCount)
            {
                DeliveryCount = deliveryCount;
                ConcurrentDeliveryCount = concurrentDeliveryCount;
            }

            public long DeliveryCount { get; }
            public int ConcurrentDeliveryCount { get; }
        }
    }
}