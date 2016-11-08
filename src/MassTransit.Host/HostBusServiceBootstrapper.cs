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
    using Autofac;
    using Topshelf;


    public abstract class HostBusServiceBootstrapper :
        IServiceBootstrapper
    {
        readonly ILifetimeScope _lifetimeScope;
        readonly string _lifetimeScopeTag;
        readonly string _serviceName;

        protected HostBusServiceBootstrapper(ILifetimeScope lifetimeScope, Type bootstrapperType)
        {
            _lifetimeScope = lifetimeScope;
            _serviceName = bootstrapperType.GetServiceDescription();
            _lifetimeScopeTag = $"service_{_serviceName}";
        }

        public string ServiceName => _serviceName;

        public string LifetimeScopeTag => _lifetimeScopeTag;

        public ServiceControl CreateService()
        {
            ILifetimeScope lifetimeScope = _lifetimeScope.BeginLifetimeScope(_lifetimeScopeTag, ConfigureLifetimeScope);

            var serviceControl = lifetimeScope.Resolve<ServiceControl>();

            return new LifetimeScopeServiceControl(lifetimeScope, serviceControl, _serviceName);
        }

        protected ILifetimeScope ParentLifetimeScope => _lifetimeScope;

        protected virtual void ConfigureLifetimeScope(ContainerBuilder builder)
        {
            builder.RegisterAutofacConsumerFactory();

            builder.RegisterType<HostBusService>()
                .InstancePerServiceScope(this)
                .WithParameter(TypedParameter.From(_serviceName))
                .As<ServiceControl>();
        }
    }
}