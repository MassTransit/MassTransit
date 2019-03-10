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
namespace MassTransit.AspNetCoreIntegration.HealthChecks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Util;


    public class BusHealthCheck :
        IBusObserver,
        IHealthCheck
    {
        string _failureMessage = "";
        bool _healthy;

        public Task CreateFaulted(Exception exception)
        {
            return Failure(exception.Message);
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            return Success();
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return Failure(exception.Message);
        }

        public Task PreStop(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task PostStop(IBus bus)
        {
            return Failure("Bus has stopped");
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return Failure(exception.Message);
        }

        public Task PreStart(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task PostCreate(IBus bus)
        {
            return TaskUtil.Completed;
        }

        Task<HealthCheckResult> IHealthCheck.CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(_healthy
                ? HealthCheckResult.Healthy("MassTransit bus is ready")
                : HealthCheckResult.Unhealthy($"MassTransit bus is not ready: {_failureMessage}"));
        }

        Task Failure(string message)
        {
            _healthy = false;
            _failureMessage = message;

            return TaskUtil.Completed;
        }

        Task Success()
        {
            _healthy = true;
            _failureMessage = "";

            return TaskUtil.Completed;
        }
    }
}
