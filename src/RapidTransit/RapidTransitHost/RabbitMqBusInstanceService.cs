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
namespace RapidTransit
{
    using System;
    using MassTransit;
    using MassTransit.Logging;
    using MassTransit.RabbitMqTransport;
    using Topshelf;


    /// <summary>
    /// A service that hosts a bus instance, and supports the configuration of that bus instance at startup
    /// </summary>
    public class RabbitMqBusInstanceService :
        ServiceControl
    {
        readonly RabbitMqHostSettings _hostSettings;
        readonly BusServiceConfigurator _serviceConfigurator;
        readonly string _serviceName;
        IBusControl _busControl;
        BusHandle _busHandle;
        ILog _log;

        public RabbitMqBusInstanceService(BusServiceConfigurator serviceConfigurator, string serviceName, RabbitMqHostSettings hostSettings)
        {
            _log = Logger.Get(GetType());
            _serviceConfigurator = serviceConfigurator;
            _serviceName = serviceName;
            _hostSettings = hostSettings;
        }

        public bool Start(HostControl hostControl)
        {
            OnStarting(hostControl);

            if (_log.IsInfoEnabled)
                _log.InfoFormat("Creating bus for hosted service: {0}", _serviceName);

            try
            {
                _busControl = Bus.Factory.CreateUsingRabbitMq(configurator =>
                {
                    var host = configurator.Host(_hostSettings);

                    var serviceConfigurator = new RabbitMqServiceConfigurator(configurator, host);

                    _serviceConfigurator.Configure(serviceConfigurator);
                });

                _busHandle = _busControl.Start();

                if (_log.IsInfoEnabled)
                    _log.InfoFormat("Created bus for hosted service: {0}", _serviceName);

                OnStarted(hostControl);

                return true;
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.Error($"Creating bus for hosted service: {_serviceName}", ex);

                OnStartFailed(hostControl, ex);
                throw;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            OnStopping(hostControl);

            if (_log.IsInfoEnabled)
                _log.InfoFormat("Stopping bus for hosted service: {0}", _serviceName);

            try
            {
                //       Parallel.ForEach(_instances, instance => instance.Dispose());


                OnStopped(hostControl);

                if (_log.IsInfoEnabled)
                    _log.InfoFormat("Stopping bus for hosted service: {0}", _serviceName);
            }
            catch (Exception ex)
            {
                OnStopFailed(hostControl, ex);
                throw;
            }

            return true;
        }

        protected virtual void OnStarting(HostControl hostControl)
        {
        }

        protected virtual void OnStarted(HostControl hostControl)
        {
        }

        protected virtual void OnStartFailed(HostControl hostControl, Exception exception)
        {
        }

        protected virtual void OnStopping(HostControl hostControl)
        {
        }

        protected virtual void OnStopped(HostControl hostControl)
        {
        }

        protected virtual void OnStopFailed(HostControl hostControl, Exception exception)
        {
        }
    }
}