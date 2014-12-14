// Copyright 2011 Chris Patterson, Dru Sellers
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
namespace Automatonymous
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using MassTransit.Saga;


    public class AutomatonymousStateMachinePolicyFactory<TInstance> :
        StateMachinePolicyFactory<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        readonly StateMachine<TInstance> _stateMachine;

        public AutomatonymousStateMachinePolicyFactory(StateMachine<TInstance> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public ISagaPolicy<TInstance, TMessage> GetPolicy<TMessage>(IEnumerable<State> states,
                                                                    Func<TMessage, Guid> getNewSagaId,
                                                                    Expression<Func<TInstance, bool>> removeExpression)
            where TMessage : class
        {
            bool includesInitial = false;
            bool includesOther = false;

            foreach (State state in states)
            {
                if (IsInitial(state))
                    includesInitial = true;
                else
                    includesOther = true;
            }

            if (includesInitial && includesOther)
                return new CreateOrUseExistingSagaPolicy<TInstance, TMessage>(getNewSagaId, removeExpression);

            if (includesInitial)
                return new InitiatingSagaPolicy<TInstance, TMessage>(getNewSagaId, removeExpression);

            return new ExistingOrIgnoreSagaPolicy<TInstance, TMessage>(removeExpression);
        }

        bool IsInitial(State state)
        {
            return state == _stateMachine.Initial;
        }
    }
}