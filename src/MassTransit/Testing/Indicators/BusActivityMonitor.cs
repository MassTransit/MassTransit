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
namespace MassTransit.Testing.Indicators
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Signalable resource which monitors bus activity.
    /// </summary>
    public class BusActivityMonitor : ISignalResource,
        IBusActivityMonitor
    {
        readonly SemaphoreSlim _activityEvent = new SemaphoreSlim(0, 1);

        Task IBusActivityMonitor.AwaitBusInactivity()
        {
            return _activityEvent.WaitAsync();
        }

        Task<bool> IBusActivityMonitor.AwaitBusInactivity(TimeSpan timeout)
        {
            return _activityEvent.WaitAsync(timeout);
        }

        Task IBusActivityMonitor.AwaitBusInactivity(CancellationToken cancellationToken)
        {
            return _activityEvent.WaitAsync(cancellationToken);
        }

        void ISignalResource.Signal()
        {
            try
            {
                _activityEvent.Release();
            }
            catch (SemaphoreFullException)
            {
            }
        }
    }
}