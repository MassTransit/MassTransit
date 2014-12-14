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
    using System.Linq.Expressions;


    public class CreateOrUseExistingSagaPolicy<TSaga, TMessage> :
        ISagaPolicy<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly Func<TSaga, bool> _canRemoveInstance;
        readonly Func<TMessage, Guid> _getNewSagaId;

        public CreateOrUseExistingSagaPolicy(Func<TMessage, Guid> getNewSagaId, Expression<Func<TSaga, bool>> removeExpression)
        {
            _getNewSagaId = getNewSagaId;
            _canRemoveInstance = removeExpression.Compile();
        }

        public bool CanCreateInstance(ConsumeContext<TMessage> context)
        {
            return true;
        }

        public TSaga CreateInstance(ConsumeContext<TMessage> context, Guid sagaId)
        {
            // TODO get the optimized saga factory for this one
            return (TSaga)Activator.CreateInstance(typeof(TSaga), sagaId);
        }

        public bool CanUseExistingInstance(ConsumeContext<TMessage> context)
        {
            return true;
        }

        public bool CanRemoveInstance(TSaga instance)
        {
            return _canRemoveInstance(instance);
        }

        public Guid GetNewSagaId(ConsumeContext<TMessage> context)
        {
            Guid sagaId = _getNewSagaId(context.Message);

            return sagaId;
        }
    }
}