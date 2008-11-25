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
namespace MassTransit.Tests.StateMachine
{
    public class StateEventBuilder<T>
    {
        private readonly State<T> _state;
        private readonly StateEvent<T> _stateEvent;

        public StateEventBuilder(State<T> state, StateEvent<T> stateEvent)
        {
            _state = state;
            _stateEvent = stateEvent;
        }

        public State<T> TransitionTo(State<T> state)
        {
            StateTransition<T> transition = new StateTransition<T>(state);

            _state.AddTransition(_stateEvent, transition);

            return _state;
        }
    }
}