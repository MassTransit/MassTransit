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
    public class OrderState : StateMachineBase<OrderState>
    {
        static OrderState()
        {
            Idle = Idle.Define();
            Active = Active.Define();
            OrderPaid = OrderPaid.Define();

            OrderReceived = OrderReceived.Define();
            PaymentReceived = PaymentReceived.Define();

            Initial = Idle;

            Idle
                .On(OrderReceived).TransitionTo(Active)
                .WhenLeaving(x => x.Called = true);
            ;

            Active
                .On(PaymentReceived).TransitionTo(OrderPaid)
                ;
        }

        public static State<OrderState> Idle { get; private set; }
        public static State<OrderState> Active { get; private set; }
        public static State<OrderState> OrderPaid { get; private set; }

        public static StateEvent<OrderState> OrderReceived { get; private set; }
        public static StateEvent<OrderState> PaymentReceived { get; private set; }

        public bool Called { get; set; }
    }
}