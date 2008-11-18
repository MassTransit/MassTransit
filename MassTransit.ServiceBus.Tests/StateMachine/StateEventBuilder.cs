// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.ServiceBus.Tests.StateMachine
{
    public class StateEventBuilder
    {
        private readonly State _state;
        private readonly StateEvent _stateEvent;

        public StateEventBuilder(State state, StateEvent stateEvent)
        {
            _state = state;
            _stateEvent = stateEvent;
        }

        public void TransitionTo(State state)
        {
            StateTransition transition = new StateTransition(state);

            _state.AddTransition(_stateEvent, transition);
        }
    }
}