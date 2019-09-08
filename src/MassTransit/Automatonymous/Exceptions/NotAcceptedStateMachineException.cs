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
namespace Automatonymous
{
    using System;
    using MassTransit;


    [Serializable]
    public class NotAcceptedStateMachineException :
        SagaException
    {
        public NotAcceptedStateMachineException(Type sagaType, Type messageType, Guid correlationId, string currentState, Exception exception)
            : base($"Not accepted in state {currentState}", sagaType, messageType, correlationId, exception)
        {
        }
    }
}