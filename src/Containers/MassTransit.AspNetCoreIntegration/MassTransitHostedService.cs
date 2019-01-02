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
namespace MassTransit.AspNetCoreIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using HealthChecks;
    using Logging.Tracing;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;


    public class MassTransitHostedService : IHostedService
    {
        readonly IBusControl _bus;
        readonly SimplifiedBusHealthCheck _simplifiedBusCheck;
        readonly ReceiveEndpointHealthCheck _receiveEndpointCheck;

        public MassTransitHostedService(IBusControl bus, ILoggerFactory loggerFactory, SimplifiedBusHealthCheck simplifiedBusCheck,
            ReceiveEndpointHealthCheck receiveEndpointCheck)
        {
            _bus = bus;
            _simplifiedBusCheck = simplifiedBusCheck;
            _receiveEndpointCheck = receiveEndpointCheck;
            if (loggerFactory != null && Logging.Logger.Current.GetType() == typeof(TraceLogger))
                ExtensionsLoggingIntegration.ExtensionsLogger.Use(loggerFactory);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _bus.ConnectReceiveEndpointObserver(_receiveEndpointCheck);

            await _bus.StartAsync(cancellationToken);

            _simplifiedBusCheck.ReportBusStarted();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _bus.StopAsync(cancellationToken);
        }
    }
}