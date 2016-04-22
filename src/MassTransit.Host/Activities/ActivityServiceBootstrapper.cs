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
namespace MassTransit.Host.Activities
{
    using Autofac;
    using AutofacIntegration;
    using Courier;
    using Topshelf;


    public abstract class ActivityServiceBootstrapper<TActivity, TArguments, TLog> :
        IServiceBootstrapper
        where TActivity : class, Activity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly ILifetimeScope _lifetimeScope;
        readonly string _serviceName;

        protected ActivityServiceBootstrapper(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;

            _serviceName = typeof(TActivity).GetServiceDescription();

            LifetimeScopeTag = $"service_{_serviceName}";
        }

        public ServiceControl CreateService()
        {
            ILifetimeScope lifetimeScope = _lifetimeScope.BeginLifetimeScope(ConfigureLifetimeScope);

            var serviceControl = lifetimeScope.Resolve<ServiceControl>();

            return new LifetimeScopeServiceControl(lifetimeScope, serviceControl, _serviceName);
        }

        public string ServiceName => _serviceName;

        public string LifetimeScopeTag { get; }

        protected virtual void ConfigureLifetimeScope(ContainerBuilder builder)
        {
            builder.RegisterType<TActivity>();

            builder.RegisterType<AutofacExecuteActivityFactory<TActivity, TArguments>>()
                .As<ExecuteActivityFactory<TArguments>>();

            builder.RegisterType<AutofacCompensateActivityFactory<TActivity, TLog>>()
                .As<CompensateActivityFactory<TLog>>();

            builder.RegisterType<ActivityService<TActivity, TArguments, TLog>>()
                .As<ServiceControl>();
        }
    }
}