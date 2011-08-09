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
    using System.Reactive.Linq;

    public static class ServiceBusExtensions
    {
        public static IObservable<T> AsObservable<T>(this IServiceBus bus) where T : class
        {
            return Observable.Create<T>(
                observer => new ServiceBusSubscription<T>(bus, observer, null));
        }

        public static IObservable<T> AsObservable<T>(this IServiceBus bus, Predicate<T> condition) where T : class
        {
            return Observable.Create<T>(
                observer => new ServiceBusSubscription<T>(bus, observer, condition));
        }
    }
}