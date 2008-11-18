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

    public class State<T>
    {
        private readonly Dictionary<StateEvent<T>, StateTransition<T>> _transitions = new Dictionary<StateEvent<T>, StateTransition<T>>();
        private readonly List<Action<T>> _enterActions = new List<Action<T>>();
        private readonly List<Action<T>> _leaveActions = new List<Action<T>>();

        public int TransitionCount
        {
            get { return _transitions.Count; }
        }

        public void AddTransition(StateEvent<T> stateEvent, StateTransition<T> transition)
        {
            _transitions.Add(stateEvent, transition);
        }

        public State<T> Handle(StateEvent<T> stateEvent)
        {
            StateTransition<T> transition;
            if (_transitions.TryGetValue(stateEvent, out transition))
            {
                return transition.Execute(stateEvent);
            }

            throw new StateException("Unhandled event in this state: " + stateEvent);
        }

        public void Enter(T state)
        {
            foreach (Action<T> action in _enterActions)
            {
                action(state);
            }
        }

        public State<T> WhenEntering(params Action<T>[] actions)
        {
            _enterActions.AddRange(actions);

            return this;
        }

        public State<T> WhenLeaving(params Action<T>[] actions)
        {
            _leaveActions.AddRange(actions);

            return this;
        }

        public void Leave(T state)
        {
            foreach (Action<T> action in _leaveActions)
            {
                action(state);
            }
        }
    }
}