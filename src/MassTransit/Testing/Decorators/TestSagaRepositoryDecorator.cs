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
namespace MassTransit.Testing.Decorators
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MessageObservers;
    using Saga;


    public class TestSagaRepositoryDecorator<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly SagaList<TSaga> _created;
        readonly ReceivedMessageList _received;
        readonly ISagaRepository<TSaga> _sagaRepository;
        readonly SagaList<TSaga> _sagas;

        public TestSagaRepositoryDecorator(ISagaRepository<TSaga> sagaRepository, ReceivedMessageList received, SagaList<TSaga> created,
            SagaList<TSaga> sagas)
        {
            _sagaRepository = sagaRepository;
            _received = received;
            _created = created;
            _sagas = sagas;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _sagaRepository.Probe(context);
        }

        Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            var interceptPipe = new InterceptPipe<T>(_sagas, _received, next);
            var interceptPolicy = new InterceptPolicy<T>(_created, policy);

            return _sagaRepository.Send(context, interceptPolicy, interceptPipe);
        }

        Task ISagaRepository<TSaga>.SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            var interceptPipe = new InterceptPipe<T>(_sagas, _received, next);
            var interceptPolicy = new InterceptPolicy<T>(_created, policy);

            return _sagaRepository.SendQuery(context, query, interceptPolicy, interceptPipe);
        }


        class InterceptPipe<TMessage> :
            IPipe<SagaConsumeContext<TSaga, TMessage>>
            where TMessage : class
        {
            readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _pipe;
            readonly ReceivedMessageList _received;
            readonly SagaList<TSaga> _sagas;

            public InterceptPipe(SagaList<TSaga> sagas, ReceivedMessageList received, IPipe<SagaConsumeContext<TSaga, TMessage>> pipe)
            {
                _sagas = sagas;
                _received = received;
                _pipe = pipe;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                 _pipe.Probe(context);
            }

            public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
            {
                _sagas.Add(context);

                try
                {
                    await _pipe.Send(context).ConfigureAwait(false);

                    _received.Add(context);
                }
                catch (Exception ex)
                {
                    _received.Add(context, ex);
                    throw;
                }
            }
        }


        class InterceptPolicy<TMessage> :
            ISagaPolicy<TSaga, TMessage>
            where TMessage : class
        {
            readonly SagaList<TSaga> _created;
            readonly ISagaPolicy<TSaga, TMessage> _policy;

            public InterceptPolicy(SagaList<TSaga> created, ISagaPolicy<TSaga, TMessage> policy)
            {
                _created = created;
                _policy = policy;
            }

            public bool PreInsertInstance(ConsumeContext<TMessage> context, out TSaga instance)
            {
                return _policy.PreInsertInstance(context, out instance);
            }

            public Task Existing(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
            {
                return _policy.Existing(context, next);
            }

            public Task Missing(ConsumeContext<TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
            {
                var interceptPipe = new InterceptPolicyPipe(_created, next);

                return _policy.Missing(context, interceptPipe);
            }


            class InterceptPolicyPipe :
                IPipe<SagaConsumeContext<TSaga, TMessage>>
            {
                readonly SagaList<TSaga> _created;
                readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _pipe;

                public InterceptPolicyPipe(SagaList<TSaga> created, IPipe<SagaConsumeContext<TSaga, TMessage>> pipe)
                {
                    _created = created;
                    _pipe = pipe;
                }

                void IProbeSite.Probe(ProbeContext context)
                {
                    _pipe.Probe(context);
                }

                public Task Send(SagaConsumeContext<TSaga, TMessage> context)
                {
                    _created.Add(context);

                    return _pipe.Send(context);
                }
            }
        }
    }
}