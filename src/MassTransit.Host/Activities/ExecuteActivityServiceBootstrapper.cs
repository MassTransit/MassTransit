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


    public abstract class ExecuteActivityServiceBootstrapper<TActivity, TArguments> :
        IServiceBootstrapper
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly ILifetimeScope _lifetimeScope;
        readonly string _lifetimeScopeTag;
        readonly string _serviceName;

        protected ExecuteActivityServiceBootstrapper(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;

            _serviceName = typeof(TActivity).GetServiceDescription();
            _lifetimeScopeTag = string.Format("service_{0}", _serviceName);
        }

        public string ServiceName
        {
            get { return _serviceName; }
        }

        public string LifetimeScopeTag
        {
            get { return _lifetimeScopeTag; }
        }

        public ServiceControl CreateService()
        {
            ILifetimeScope lifetimeScope = _lifetimeScope.BeginLifetimeScope(ConfigureLifetimeScope);

            var serviceControl = lifetimeScope.Resolve<ServiceControl>();

            return new LifetimeScopeServiceControl(lifetimeScope, serviceControl, _serviceName);
        }

        protected virtual void ConfigureLifetimeScope(ContainerBuilder builder)
        {
            builder.RegisterType<TActivity>();

            builder.RegisterType<AutofacExecuteActivityFactory<TActivity, TArguments>>()
                .As<ExecuteActivityFactory<TArguments>>();

            builder.RegisterType<ExecuteActivityService<TActivity, TArguments>>()
                .As<ServiceControl>();
        }
    }
}