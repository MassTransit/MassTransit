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
        private readonly List<Action<State>> _commands = new List<Action<State>>();

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

        public void AddCommand(Action<State> action)
        {
            _commands.Add(action);
        }

        public void Execute(State state)
        {
            foreach (Action<State> command in _commands)
            {
                command(state);
            }
        }
    }
}