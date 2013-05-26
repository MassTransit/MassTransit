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
namespace MassTransit
{
    using System;
    using Magnum.Reflection;
    using Ninject;
    using NinjectIntegration;
    using Saga;
    using SubscriptionConfigurators;
    using Util;


    public class NinjectSagaFactoryConfigurator
    {
        readonly SubscriptionBusServiceConfigurator _configurator;
        readonly IKernel _kernel;

        public NinjectSagaFactoryConfigurator(SubscriptionBusServiceConfigurator configurator, IKernel kernel)
        {
            _kernel = kernel;
            _configurator = configurator;
        }

        public void ConfigureSaga(Type sagaType)
        {
            this.FastInvoke(new[] {sagaType}, "Configure");
        }

        [UsedImplicitly]
        public void Configure<T>()
            where T : class, ISaga
        {
            var sagaRepository = _kernel.Get<ISagaRepository<T>>();

            var ninjectSagaRepository = new NinjectSagaRepository<T>(sagaRepository, _kernel);

            _configurator.Saga(ninjectSagaRepository);
        }
    }
}