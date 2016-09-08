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
namespace MassTransit.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes.Util;
    using Util;


    public class BusObservable :
        Connectable<IBusObserver>,
        IBusObserver
    {
        public Task PostCreate(IBus bus)
        {
            return ForEachAsync(x => x.PostCreate(bus));
        }

        public Task CreateFaulted(Exception exception)
        {
            return ForEachAsync(x => x.CreateFaulted(exception));
        }

        public Task PreStart(IBus bus)
        {
            return ForEachAsync(x => x.PreStart(bus));
        }

        public Task PostStart(IBus bus, Task busReady)
        {
            return ForEachAsync(x => x.PostStart(bus, busReady));
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return ForEachAsync(x => x.StartFaulted(bus, exception));
        }

        public Task PreStop(IBus bus)
        {
            return ForEachAsync(x => x.PreStop(bus));
        }

        public Task PostStop(IBus bus)
        {
            return ForEachAsync(x => x.PostStop(bus));
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return ForEachAsync(x => x.StopFaulted(bus, exception));
        }
    }
}