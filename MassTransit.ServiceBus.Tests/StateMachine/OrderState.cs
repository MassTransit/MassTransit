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
            Define(() => Idle);
            Define(() => Active);
            Define(() => OrderPaid);

            Define(() => OrderReceived);
            Define(() => PaymentReceived);

            Initial = Idle;

            Idle
                .On(OrderReceived).TransitionTo(Active)
                .WhenLeaving(x => x.Called = true);
            ;

            Active
                .On(PaymentReceived).TransitionTo(OrderPaid)
                ;
        }

        public static State<OrderState> Idle { get; set; }
        public static State<OrderState> Active { get; set; }
        public static State<OrderState> OrderPaid { get; set; }

        public static StateEvent<OrderState> OrderReceived { get; set; }
        public static StateEvent<OrderState> PaymentReceived { get; set; }

        public bool Called { get; set; }
    }
}