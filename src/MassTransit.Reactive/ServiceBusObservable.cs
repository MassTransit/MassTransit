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
namespace MassTransit.Reactive
{
    using System;

    public class ServiceBusObservable<T> : 
        IObservable<T> where T : class
    {
        readonly IServiceBus _bus;
        readonly Predicate<T> _condition;

        public ServiceBusObservable(IServiceBus bus)
            : this(bus, null)
        {
        }

        public ServiceBusObservable(IServiceBus bus, Predicate<T> condition)
        {
            _bus = bus;
            _condition = condition;
        }


        public IDisposable Subscribe(IObserver<T> observer)
        {
            return new ServiceBusSubscription<T>(_bus, observer, _condition);
        }
    }
}