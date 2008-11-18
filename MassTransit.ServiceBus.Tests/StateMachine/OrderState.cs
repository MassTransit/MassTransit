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
    public class OrderState : StateMachineBase
    {
        static OrderState()
        {
            Idle = State.Define();
            Active = State.Define();
            OrderPaid = State.Define();

            OrderReceived = StateEvent.Define();
            PaymentReceived = StateEvent.Define();

            Initial = Idle;

            Idle
                .On(OrderReceived).TransitionTo(Active)
                ;

            Active
                .On(PaymentReceived).TransitionTo(OrderPaid)
                ;
        }

        public static State Idle { get; private set; }
        public static State Active { get; private set; }
        public static State OrderPaid { get; private set; }

        public static StateEvent OrderReceived { get; private set; }
        public static StateEvent PaymentReceived { get; private set; }
    }
}