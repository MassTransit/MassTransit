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
namespace MassTransit.QuartzIntegration.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Logging;
    using Quartz;
    using Util;


    /// <summary>
    /// Used to start and stop an in-memory scheduler using Quartz
    /// </summary>
    public class InMemorySchedulerBusObserver :
        IBusObserver
    {
        readonly ILog _log = Logger.Get<InMemorySchedulerBusObserver>();
        readonly IScheduler _scheduler;

        public InMemorySchedulerBusObserver(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public Task PostCreate(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task CreateFaulted(Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task PreStart(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public async Task PostStart(IBus bus, Task busReady)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Quartz Scheduler Starting: {0} ({1}/{2})", bus.Address, _scheduler.SchedulerName, _scheduler.SchedulerInstanceId);

            await busReady;

            _scheduler.JobFactory = new MassTransitJobFactory(bus);
            _scheduler.Start();

            if (_log.IsInfoEnabled)
                _log.InfoFormat("Quartz Scheduler Started: {0} ({1}/{2})", bus.Address, _scheduler.SchedulerName, _scheduler.SchedulerInstanceId);
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task PreStop(IBus bus)
        {
            _scheduler.Standby();

            if (_log.IsInfoEnabled)
                _log.InfoFormat("Quartz Scheduler Paused: {0} ({1}/{2})", bus.Address, _scheduler.SchedulerName, _scheduler.SchedulerInstanceId);

            return TaskUtil.Completed;
        }

        public Task PostStop(IBus bus)
        {
            _scheduler.Shutdown();

            if (_log.IsInfoEnabled)
                _log.InfoFormat("Quartz Scheduler Stopped: {0} ({1}/{2})", bus.Address, _scheduler.SchedulerName, _scheduler.SchedulerInstanceId);

            return TaskUtil.Completed;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }
    }
}