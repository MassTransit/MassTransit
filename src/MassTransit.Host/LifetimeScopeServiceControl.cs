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
    using Autofac;
    using Topshelf;


    /// <summary>
    /// Decorates ServiceControl and manages the LifetimeScope for the service as part of the Start/Stop handshake.
    /// If the service is stopped, the LifetimeScope is disposed. The service cannot be restarted.
    /// </summary>
    public class LifetimeScopeServiceControl :
        ServiceControl
    {
        readonly ILifetimeScope _lifetimeScope;
        readonly ServiceControl _service;
        readonly string _serviceName;

        public LifetimeScopeServiceControl(ILifetimeScope lifetimeScope, ServiceControl service, string serviceName)
        {
            _lifetimeScope = lifetimeScope;
            _service = service;
            _serviceName = serviceName;
        }

        public bool Start(HostControl hostControl)
        {
            return _service.Start(hostControl);
        }

        public bool Stop(HostControl hostControl)
        {
            bool stop = _service.Stop(hostControl);
            if (stop)
                _lifetimeScope.Dispose();
            return stop;
        }

        public override string ToString()
        {
            return _serviceName;
        }
    }
}