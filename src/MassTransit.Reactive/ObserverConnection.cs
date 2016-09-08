// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Reactive;
    using GreenPipes;


    public class ObserverConnection<T> :
        IDisposable
        where T : class
    {
        readonly ConnectHandle _handle;
        readonly IObserver<ConsumeContext<T>> _observer;

        public ObserverConnection(IBus bus, IObserver<T> observer)
        {
            _observer = Observer.Synchronize(new ConsumeObserver<T>(observer));

            _handle = bus.ConnectObserver(_observer);
        }

        public ObserverConnection(IBus bus, IObserver<ConsumeContext<T>> observer)
        {
            _observer = Observer.Synchronize(observer);

            _handle = bus.ConnectObserver(_observer);
        }

        public void Dispose()
        {
            _observer.OnCompleted();
            _handle.Dispose();
        }
    }
}