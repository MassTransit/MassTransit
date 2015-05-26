namespace RapidTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Logging;
    using Topshelf;


    public class ServiceBusInstanceService :
        ServiceControl,
        IDisposable
    {
        readonly IServiceBusInstance[] _instances;
        readonly string _serviceName;
        readonly IServiceConfigurator _serviceConfigurator;
        bool _disposed;

        public ServiceBusInstanceService(IServiceConfigurator serviceConfigurator, IEnumerable<IServiceBusInstance> instances,
            string serviceName)
        {
            _serviceConfigurator = serviceConfigurator;
            _serviceName = serviceName;
            _instances = instances.ToArray();
        }

        public virtual void Dispose()
        {
            if (_disposed)
                return;

//            Parallel.ForEach(_instances, x => x.Dispose());

            _disposed = true;
        }

        public bool Start(HostControl hostControl)
        {
            OnStarting(hostControl);

            Logger.Get(GetType())
                  .InfoFormat("Creating {0} Service Buses for hosted service: {1}", _instances.Length, _serviceName);

            try
            {
//                Parallel.ForEach(_instances, instance => instance.Start(_transportConfigurator));

                OnStarted(hostControl);

                Logger.Get(GetType())
                      .InfoFormat("Created {0} Service Buses for hosted service: {1}", _instances.Length, _serviceName);

                return true;
            }
            catch (Exception ex)
            {
//                Parallel.ForEach(_instances, instance => instance.Dispose());

                OnStartFailed(hostControl, ex);
                throw;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            OnStopping(hostControl);

            Logger.Get(GetType())
                  .InfoFormat("Stopping {0} Service Buses for hosted service: {1}", _instances.Length, _serviceName);

            try
            {
         //       Parallel.ForEach(_instances, instance => instance.Dispose());

                _disposed = true;

                OnStopped(hostControl);

                Logger.Get(GetType())
                      .InfoFormat("Stopped {0} Service Buses for hosted service: {1}", _instances.Length, _serviceName);
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