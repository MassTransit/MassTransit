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
namespace MassTransit.Host
{
    using System;
    using System.Diagnostics;
    using Hosting;
    using Logging;
    using Microsoft.Extensions.Logging;
    using Topshelf;
    using Util;


    /// <summary>
    /// A service that hosts a bus instance, and supports the configuration of that bus instance at startup
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay()}")]
    public class HostBusService :
        ServiceControl
    {
        readonly IHostBusFactory _hostBusFactory;
        readonly ILogger _logger;
        readonly IBusServiceConfigurator _serviceConfigurator;
        readonly string _serviceName;
        IBusControl _busControl;
        BusHandle _busHandle;

        public HostBusService(IHostBusFactory hostBusFactory, IBusServiceConfigurator serviceConfigurator, string serviceName)
        {
            _logger = Logger.Get(GetType());
            _hostBusFactory = hostBusFactory;
            _serviceConfigurator = serviceConfigurator;
            _serviceName = serviceName;
        }

        public bool Start(HostControl hostControl)
        {
            _logger.LogInformation("Creating bus for hosted service: {0}", _serviceName);

            try
            {
                _busControl = _hostBusFactory.CreateBus(_serviceConfigurator, _serviceName);

                _busHandle = TaskUtil.Await(() => _busControl.StartAsync());

                _logger.LogInformation("Created bus for hosted service: {0}", _serviceName);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Creating bus for hosted service: {_serviceName}", ex);

                throw;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            _logger.LogInformation("Stopping bus for hosted service: {0}", _serviceName);

            _busHandle?.Stop(TimeSpan.FromSeconds(60));

            _logger.LogInformation("Stopping bus for hosted service: {0}", _serviceName);

            return true;
        }

        string DebuggerDisplay()
        {
            return _serviceName;
        }
    }
}
