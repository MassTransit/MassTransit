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
    public class StateBuilder<T> where T : StateMachineBase<T>
    {
        private readonly State<T> _state;

        public StateBuilder(State<T> state)
        {
            _state = state;
        }

        public static implicit operator State<T>(StateBuilder<T> builder)
        {
            return builder._state;
        }

        public void AsInitial()
        {
            StateMachineBase<T>.Initial = _state;
        }
    }
}