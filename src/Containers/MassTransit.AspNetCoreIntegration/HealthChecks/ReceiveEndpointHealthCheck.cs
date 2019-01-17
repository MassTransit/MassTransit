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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;


    public class ReceiveEndpointHealthCheck : IReceiveEndpointObserver,
        IHealthCheck
    {
        readonly Dictionary<string, EndpointStatus> _endpoints = new Dictionary<string, EndpointStatus>();

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            if (_endpoints.All(x => x.Value.Ready))
                return Task.FromResult(HealthCheckResult.Healthy("All endpoints are ready"));

            var faulted = string.Join(",", _endpoints.Where(x => !x.Value.Ready).Select(x => x.Key));

            return Task.FromResult(HealthCheckResult.Unhealthy($"Failed endpoints: {faulted}"));
        }

        public Task Ready(ReceiveEndpointReady ready)
        {
            GetEndpoint(ready.InputAddress).Ready = true;
            return Health.Done;
        }

        public Task Completed(ReceiveEndpointCompleted completed)
        {
            return Health.Done;
        }

        public Task Faulted(ReceiveEndpointFaulted faulted)
        {
            var endpoint = GetEndpoint(faulted.InputAddress);

            endpoint.Ready = false;
            endpoint.LastException = faulted.Exception;

            return Health.Done;
        }

        EndpointStatus GetEndpoint(Uri inputAddress)
        {
            var address = inputAddress.ToString();

            if (!_endpoints.ContainsKey(address))
                _endpoints.Add(address, new EndpointStatus());

            return _endpoints[address];
        }


        class EndpointStatus
        {
            public bool Ready { get; set; }
            public Exception LastException { get; set; }
        }
    }
}