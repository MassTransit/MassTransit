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
    using NUnit.Framework;

    [TestFixture]
    public class StateHelpers_Specs
    {
        [Test, ExpectedException(typeof (StateException))]
        public void An_unexptected_change_should_throw()
        {
            OrderState state = new OrderState();

            state.Handle(OrderState.PaymentReceived);

            Assert.Fail("We should have thrown an exception");
        }

        [Test]
        public void Commands_should_be_allowed_on_states()
        {
            OrderState state = new OrderState();

            bool invoked = false;

            OrderState.Active.WhenEntering(x => { invoked = true; });

            state.Handle(OrderState.OrderReceived);

            Assert.AreEqual(true, invoked);
        }

        [Test]
        public void Commands_should_be_allowed_when_a_state_is_left()
        {
            OrderState state = new OrderState();

            bool invoked = false;

            OrderState.Idle.WhenLeaving(x => { invoked = true; });

            state.Handle(OrderState.OrderReceived);

            Assert.AreEqual(true, invoked);
        }

        [Test]
        public void I_should_have_the_appropriate_transitions_defined()
        {
            Assert.AreEqual(1, OrderState.Idle.TransitionCount);
        }

        [Test]
        public void The_initial_state_should_be_idle()
        {
            OrderState state = new OrderState();

            Assert.AreEqual(OrderState.Idle, state.Current);
        }

        [Test]
        public void The_state_should_again_evolve_when_more_stuff_happens()
        {
            OrderState state = new OrderState();

            state.Handle(OrderState.OrderReceived);
            state.Handle(OrderState.PaymentReceived);

            Assert.AreEqual(OrderState.OrderPaid, state.Current);
        }

        [Test]
        public void The_state_should_change_when_an_event_occurs()
        {
            OrderState state = new OrderState();

            state.Handle(OrderState.OrderReceived);

            Assert.AreEqual(OrderState.Active, state.Current);
        }
    }
}