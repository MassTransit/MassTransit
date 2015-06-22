// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Saga
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using Monitoring.Introspection;


    /// <summary>
    /// Decorates a saga repository with a callback method that is invoked before every
    /// instance of the saga is returned, allowing any dependencies to be set.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class DelegatingSagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly Action<SagaConsumeContext<TSaga>> _callback;
        readonly ISagaRepository<TSaga> _repository;

        public DelegatingSagaRepository(ISagaRepository<TSaga> repository, Action<SagaConsumeContext<TSaga>> callback)
        {
            _repository = repository;
            _callback = callback;
        }

        Task IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("delegate");

            return _repository.Probe(scope);
        }

        public async Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next) where T : class
        {
            IPipe<SagaConsumeContext<TSaga, T>> callbackPipe = Pipe.New<SagaConsumeContext<TSaga, T>>(x =>
            {
                x.Execute(_callback);
                x.ExecuteAsync(next.Send);
            });

            await _repository.Send(context, policy, callbackPipe);
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next) where T : class
        {
            IPipe<SagaConsumeContext<TSaga, T>> callbackPipe = Pipe.New<SagaConsumeContext<TSaga, T>>(x =>
            {
                x.Execute(_callback);
                x.ExecuteAsync(next.Send);
            });

            await _repository.SendQuery(context, policy, callbackPipe);
        }
    }
}