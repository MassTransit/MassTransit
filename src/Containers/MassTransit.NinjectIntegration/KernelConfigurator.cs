// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.NinjectIntegration
{
    using Ninject;
    using Ninject.Extensions.NamedScope;
    using Saga;


    public class KernelConfigurator :
        IKernelConfigurator
    {
        readonly IConsumerRegistry _consumerRegistry;
        readonly IKernel _kernel;
        readonly ISagaRegistry _sagaRegistry;

        public KernelConfigurator(IKernel kernel)
        {
            _kernel = kernel;

            var namedScopeModule = new NamedScopeModule();
            if (!kernel.HasModule(namedScopeModule.Name))
                kernel.Load(namedScopeModule);

            _consumerRegistry = kernel.TryGet<IConsumerRegistry>();
            if (_consumerRegistry == null)
            {
                kernel.Bind<IConsumerRegistry>().To<ConsumerRegistry>().InSingletonScope();
                _consumerRegistry = kernel.Get<IConsumerRegistry>();
            }

            _sagaRegistry = kernel.TryGet<ISagaRegistry>();
            if (_sagaRegistry == null)
            {
                kernel.Bind<ISagaRegistry>().To<SagaRegistry>().InSingletonScope();
                _sagaRegistry = kernel.Get<ISagaRegistry>();
            }
        }

        public void AddConsumer<T>()
            where T : class, IConsumer
        {
            _kernel.Bind<T>().ToSelf().DefinesNamedScope("message");

            _consumerRegistry.Add<T>();
        }

        public void AddSaga<T>()
            where T : class, ISaga
        {
            _sagaRegistry.Add<T>();
        }
    }
}