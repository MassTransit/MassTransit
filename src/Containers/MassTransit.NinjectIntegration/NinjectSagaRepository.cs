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
namespace MassTransit.NinjectIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ninject;
    using Ninject.Activation.Blocks;
    using Pipeline;
    using Saga;


    public class NinjectSagaRepository<T> :
        ISagaRepository<T>
        where T : class, ISaga
    {
        readonly IKernel _kernel;
        readonly ISagaRepository<T> _repository;

        public NinjectSagaRepository(ISagaRepository<T> repository, IKernel kernel)
        {
            _repository = repository;
            _kernel = kernel;
        }

//        public IEnumerable<Action<IConsumeContext<TMessage>>> GetSaga<TMessage>(IConsumeContext<TMessage> context,
//            Guid sagaId, InstanceHandlerSelector<T, TMessage> selector, ISagaPolicy<T, TMessage> policy)
//            where TMessage : class
//        {
//            return _repository.GetSaga(context, sagaId, selector, policy)
//                              .Select(consumer => (Action<IConsumeContext<TMessage>>)(x =>
//                                  {
//                                      IActivationBlock activationBlock = _kernel.BeginBlock();
//
//                                      try
//                                      {
//                                          consumer(x);
//                                      }
//                                      finally
//                                      {
//                                          activationBlock.Dispose();
//                                      }
//                                  }));
//        }

        public Task Send<T1>(ConsumeContext<T1> context, IPipe<SagaConsumeContext<T, T1>> next) where T1 : class
        {
            return _repository.Send(context, next);
        }

        public IEnumerable<Guid> Find(ISagaFilter<T> filter)
        {
            return _repository.Find(filter);
        }
    }
}