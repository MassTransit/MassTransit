// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.WindsorIntegration
{
    using System;
    using Castle.Windsor;
    using Magnum.Reflection;
    using Saga;
    using SubscriptionConfigurators;
    using Util;


    public class WindsorSagaFactoryConfigurator
    {
        readonly SubscriptionBusServiceConfigurator _configurator;
        readonly IWindsorContainer _container;

        public WindsorSagaFactoryConfigurator(SubscriptionBusServiceConfigurator configurator,
            IWindsorContainer container)
        {
            _container = container;
            _configurator = configurator;
        }

        public void ConfigureSaga(Type messageType)
        {
            this.FastInvoke(new[] {messageType}, "Configure");
        }

        [UsedImplicitly]
        public void Configure<T>()
            where T : class, ISaga
        {
            var sagaRepository = _container.Resolve<ISagaRepository<T>>();

            var windsorSagaRepository = new WindsorSagaRepository<T>(sagaRepository, _container);

            _configurator.Saga(windsorSagaRepository);
        }
    }
}