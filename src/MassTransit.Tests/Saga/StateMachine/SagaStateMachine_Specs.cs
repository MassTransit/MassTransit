// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Saga.StateMachine
{
    using System;
    using Magnum.StateMachine;
    using Magnum.TestFramework;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class SagaStateMachine_Specs
    {
        [SetUp]
        public void Setup()
        {
            _transactionId = NewId.NextGuid();
            _username = "jblow";
            _password = "password1";
            _email = "jblow@yourdad.com";
            _displayName = "Joe Blow";
        }

        Guid _transactionId;
        string _username;
        string _password;
        string _email;
        string _displayName;

        [Test]
        public void The_good_times_should_roll()
        {
            var workflow = new RegisterUserStateMachine();

            workflow.CurrentState.ShouldEqual(RegisterUserStateMachine.Initial);

            workflow.Consume(new RegisterUser(_transactionId, _username, _password, _displayName, _email));

            workflow.CurrentState.ShouldEqual(RegisterUserStateMachine.WaitingForEmailValidation);

            workflow.Consume(new UserValidated(_transactionId));

            workflow.CurrentState.ShouldEqual(RegisterUserStateMachine.Completed);
        }

        [Test]
        public void The_saga_state_machine_should_add_value_for_sagas()
        {
            var workflow = new RegisterUserStateMachine();

            Assert.AreEqual(RegisterUserStateMachine.Initial, workflow.CurrentState);

            workflow.Consume(new RegisterUser(_transactionId, _username, _password, _displayName, _email));

            Assert.AreEqual(RegisterUserStateMachine.WaitingForEmailValidation, workflow.CurrentState);
        }

        [Test]
        public void The_visualizer_should_work()
        {
            var workflow = new RegisterUserStateMachine();

            StateMachineInspector.Trace(workflow);
        }
    }
}