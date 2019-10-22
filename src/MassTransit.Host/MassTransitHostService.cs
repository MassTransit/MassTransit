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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using Hosting;
    using Internals.Extensions;
    using Logging;
    using Topshelf;


    public class  MassTransitHostService :
        ServiceControl
    {
        readonly ILifetimeScope _hostScope;
        ILifetimeScope _bootstrapperScope;
        readonly List<ServiceControl> _services;

        public MassTransitHostService(ILifetimeScope hostScope)
        {
            _hostScope = hostScope;
            _services = new List<ServiceControl>();
        }

        public bool Start(HostControl hostControl)
        {
//            _log.InfoFormat($"Starting {GetType().GetDisplayName()}");

            var started = new List<ServiceControl>();

            try
            {
                var scanner = new ServiceAssemblyScanner();

                List<AssemblyRegistration> registrations = scanner.GetAssemblyRegistrations().ToList();

  //              _log.Info($"Found {registrations.Count} assembly registrations");
                foreach (var registration in registrations)
                {
    //                _log.Info($"Assembly: {registration.Assembly.GetName().Name}");
        //            foreach (var type in registration.Types)
      //                  _log.Info($"  Type: {type.GetTypeName()}");
                }

                var busFactoryType = scanner.GetHostBusFactoryType();
                if (busFactoryType == null)
                    throw new ConfigurationException("A valid transport assembly was not found.");

                _bootstrapperScope = CreateBootstrapperScope(registrations, busFactoryType);

                var bootstrappers = _bootstrapperScope.Resolve<IEnumerable<IServiceBootstrapper>>();

                List<ServiceControl> services = bootstrappers.Select(x => x.CreateService()).ToList();

                Parallel.ForEach(services, serviceControl =>
                {
                    hostControl.RequestAdditionalTime(TimeSpan.FromMinutes(1));

                    StartService(hostControl, serviceControl);

                    lock (started)
                    {
                        started.Add(serviceControl);
                    }
                });

                _services.AddRange(started);

                return true;
            }
            catch (Exception ex)
            {
                //_log.Error("Service failed to start", ex);

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
            //_log.InfoFormat("Stopping {0} services", _services.Count);

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

        ILifetimeScope CreateBootstrapperScope(IEnumerable<AssemblyRegistration> registrations, Type busFactoryType)
        {
            var scope = _hostScope.BeginLifetimeScope(builder =>
            {
                builder.RegisterType(busFactoryType)
                    .As<IHostBusFactory>();

                foreach (var registration in registrations)
                {
                    builder.RegisterType<AssemblyServiceBootstrapper>()
                        .WithParameter(TypedParameter.From(registration))
                        .As<IServiceBootstrapper>();
                }
            });

            return scope;
        }

        void StartService(HostControl hostControl, ServiceControl service)
        {
            if (hostControl == null)
                throw new ArgumentNullException(nameof(hostControl));

            if (service == null)
                return;

            //_log.InfoFormat("Starting Service {0}", service);

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
              //  _log.InfoFormat("Stopping Service {0}", service);

                if (!service.Stop(hostControl))
                    throw new TopshelfException($"Failed to stop service: {service}");
            }
            catch (Exception ex)
            {
                //_log.Error("Stop Service Failed", ex);
            }
        }
    }
}
