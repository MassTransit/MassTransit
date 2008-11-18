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
    using System;
    using System.Collections.Generic;

    public class State
    {
        private readonly Dictionary<StateEvent, StateTransition> _transitions = new Dictionary<StateEvent, StateTransition>();
        private readonly List<Action<State>> _enterActions = new List<Action<State>>();
        private readonly List<Action<State>> _leaveActions = new List<Action<State>>();

        public int TransitionCount
        {
            get { return _transitions.Count; }
        }

        public static StateBuilder Define()
        {
            return new StateBuilder();
        }

        public void AddTransition(StateEvent stateEvent, StateTransition transition)
        {
            _transitions.Add(stateEvent, transition);
        }

        public State Handle(StateEvent stateEvent)
        {
            StateTransition transition;
            if (_transitions.TryGetValue(stateEvent, out transition))
            {
                return transition.Execute(stateEvent);
            }

            throw new StateException("Unhandled event in this state: " + stateEvent);
        }

        public void Enter(State state)
        {
            foreach (Action<State> action in _enterActions)
            {
                action(state);
            }
        }

        public State WhenEntering(Action<State> action)
        {
            _enterActions.Add(action);

            return this;
        }

        public State WhenLeaving(Action<State> action)
        {
            _leaveActions.Add(action);

            return this;
        }

        public void Leave(State state)
        {
            foreach (Action<State> action in _leaveActions)
            {
                action(state);
            }
        }
    }
}