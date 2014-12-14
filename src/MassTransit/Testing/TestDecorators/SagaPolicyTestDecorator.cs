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
namespace MassTransit.Testing.TestDecorators
{
    using System;
    using Saga;


    public class SagaPolicyTestDecorator<TSaga, TMessage> :
        ISagaPolicy<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly SagaListImpl<TSaga> _created;
        readonly ISagaPolicy<TSaga, TMessage> _policy;
        readonly SagaListImpl<TSaga> _removed;

        public SagaPolicyTestDecorator(ISagaPolicy<TSaga, TMessage> policy, SagaListImpl<TSaga> created)
        {
            _policy = policy;
            _created = created;
            _removed = new SagaListImpl<TSaga>();
        }

        public bool CanCreateInstance(ConsumeContext<TMessage> context)
        {
            return _policy.CanCreateInstance(context);
        }

        public TSaga CreateInstance(ConsumeContext<TMessage> context, Guid sagaId)
        {
            TSaga instance = _policy.CreateInstance(context, sagaId);
            if (instance != null)
                _created.Add(instance);

            return instance;
        }

        public Guid GetNewSagaId(ConsumeContext<TMessage> context)
        {
            return _policy.GetNewSagaId(context);
        }

        public bool CanUseExistingInstance(ConsumeContext<TMessage> context)
        {
            return _policy.CanUseExistingInstance(context);
        }

        public bool CanRemoveInstance(TSaga instance)
        {
            bool canRemoveInstance = _policy.CanRemoveInstance(instance);
            if (canRemoveInstance)
                _removed.Add(instance);
            return canRemoveInstance;
        }
    }
}