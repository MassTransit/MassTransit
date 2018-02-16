// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System;


    public static class BusObserverExtensions
    {
        /// <summary>
        /// Connect an observer to the bus, to observe creation, start, stop, fault events.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="observer"></param>
        public static void BusObserver(this IBusFactoryConfigurator configurator, IBusObserver observer)
        {
            configurator.ConnectBusObserver(observer);
        }

        /// <summary>
        /// Connect an observer to the bus, to observe creation, start, stop, fault events.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="observerFactory">Factory to create the bus observer</param>
        public static void BusObserver<T>(this IBusFactoryConfigurator configurator, Func<T> observerFactory)
            where T : IBusObserver
        {
            configurator.ConnectBusObserver(observerFactory());
        }
    }
}