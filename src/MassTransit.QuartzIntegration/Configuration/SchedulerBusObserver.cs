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
    public class SchedulerBusObserver :
        IBusObserver
    {
        readonly ILog _log = Logger.Get<SchedulerBusObserver>();
        readonly IScheduler _scheduler;
        readonly Uri _schedulerEndpointAddress;

        public SchedulerBusObserver(IScheduler scheduler, Uri schedulerEndpointAddress)
        {
            _scheduler = scheduler;
            _schedulerEndpointAddress = schedulerEndpointAddress;
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

        public async Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Quartz Scheduler Starting: {0} ({1}/{2})", _schedulerEndpointAddress, _scheduler.SchedulerName, _scheduler.SchedulerInstanceId);

            await busReady.ConfigureAwait(false);

            _scheduler.JobFactory = new MassTransitJobFactory(bus);
            await _scheduler.Start().ConfigureAwait(false);

            if (_log.IsInfoEnabled)
                _log.InfoFormat("Quartz Scheduler Started: {0} ({1}/{2})", _schedulerEndpointAddress, _scheduler.SchedulerName, _scheduler.SchedulerInstanceId);
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }

        public async Task PreStop(IBus bus)
        {
            await _scheduler.Standby().ConfigureAwait(false);

            if (_log.IsInfoEnabled)
                _log.InfoFormat("Quartz Scheduler Paused: {0} ({1}/{2})", _schedulerEndpointAddress, _scheduler.SchedulerName, _scheduler.SchedulerInstanceId);
        }

        public async Task PostStop(IBus bus)
        {
            await _scheduler.Shutdown().ConfigureAwait(false);

            if (_log.IsInfoEnabled)
                _log.InfoFormat("Quartz Scheduler Stopped: {0} ({1}/{2})", _schedulerEndpointAddress, _scheduler.SchedulerName, _scheduler.SchedulerInstanceId);
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }
    }
}