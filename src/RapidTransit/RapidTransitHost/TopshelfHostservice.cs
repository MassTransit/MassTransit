// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Logging;
    using Topshelf;


    public class TopshelfHostService :
        ServiceControl
    {
        readonly ILog _log = Logger.Get<TopshelfHostService>();
        readonly IEnumerable<IServiceBootstrapper> _serviceBootstrappers;

        IList<ServiceControl> _services;

        public TopshelfHostService(IEnumerable<IServiceBootstrapper> serviceBootstrappers)
        {
            _serviceBootstrappers = serviceBootstrappers;
        }

        public bool Start(HostControl hostControl)
        {
            _log.InfoFormat("Starting RapidTransit Host");

            var started = new List<ServiceControl>();

            try
            {
                _services = _serviceBootstrappers.Select(x => x.CreateService()).ToList();

                _log.InfoFormat("Starting {0} services", _services.Count);

                foreach (ServiceControl activityService in _services)
                {
                    hostControl.RequestAdditionalTime(TimeSpan.FromMinutes(1));
                    StartService(hostControl, activityService);

                    started.Add(activityService);
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Error("Service failed to start", ex);

                Parallel.ForEach(started, service =>
                {
                    hostControl.RequestAdditionalTime(TimeSpan.FromMinutes(1));
                    StopService(hostControl, service);
                });

                throw;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            _log.InfoFormat("Stopping {0} services", _services.Count);

            if (_services != null)
            {
                Parallel.ForEach(_services, service =>
                {
                    hostControl.RequestAdditionalTime(TimeSpan.FromMinutes(1));
                    StopService(hostControl, service);
                });
            }

            return true;
        }

        void StartService(HostControl hostControl, ServiceControl service)
        {
            if (hostControl == null)
                throw new ArgumentNullException(nameof(hostControl));

            if (service == null)
                return;

            _log.InfoFormat("Starting Service {0}", service);

            if (!service.Start(hostControl))
                throw new TopshelfException($"Failed to start service: {service}");
        }

        void StopService(HostControl hostControl, ServiceControl service)
        {
            if (hostControl == null)
                throw new ArgumentNullException(nameof(hostControl));

            if (service == null)
                return;

            try
            {
                _log.InfoFormat("Stopping Service {0}", service);

                if (!service.Stop(hostControl))
                    throw new TopshelfException($"Failed to stop service: {service}");
            }
            catch (Exception ex)
            {
                _log.Error("Stop Service Failed", ex);
            }
        }
    }
}