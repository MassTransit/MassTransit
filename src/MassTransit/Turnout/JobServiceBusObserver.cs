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
namespace MassTransit.Turnout
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Util;


    public class JobServiceBusObserver :
        IBusObserver
    {
        readonly IJobService _jobService;

        public JobServiceBusObserver(IJobService jobService)
        {
            _jobService = jobService;
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
            LogContext.Debug?.Log("Job Service starting: {InputAddress}", _jobService.InputAddress);

            await busReady.ConfigureAwait(false);

            LogContext.Info?.Log("Job Service started: {InputAddress}", _jobService.InputAddress);
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }

        public async Task PreStop(IBus bus)
        {
            LogContext.Debug?.Log("Job Service shutting down: {InputAddress}", _jobService.InputAddress);

            await _jobService.Stop().ConfigureAwait(false);

            LogContext.Info?.Log("Job Service shut down: {InputAddress}", _jobService.InputAddress);
        }

        public Task PostStop(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }
    }
}
