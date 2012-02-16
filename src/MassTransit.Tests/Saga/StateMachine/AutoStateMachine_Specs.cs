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
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MassTransit.Pipeline.Inspectors;
    using MassTransit.Saga;
    using Messages;
    using NUnit.Framework;
    using TestFramework;
    using TextFixtures;

    [TestFixture]
    public class AutoStateMachine_Specs :
        LoopbackTestFixture
    {
        ISagaRepository<AutoStateMachineSaga> _repository;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _repository = SetupSagaRepository<AutoStateMachineSaga>();

            // this just shows that you can easily respond to the message
            LocalBus.SubscribeHandler<SendUserVerificationEmail>(
                x => LocalBus.Publish(new UserVerificationEmailSent(x.CorrelationId, x.Email)));

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
        public void A_state_machine_based_saga_should_automatically_wire_up_subscriptions()
        {
            LocalBus.SubscribeSaga(_repository);

            PipelineViewer.Trace(LocalBus.InboundPipeline);
            PipelineViewer.Trace(LocalBus.OutboundPipeline);

            LocalBus.Publish(new RegisterUser(_transactionId, _username, _password, _displayName, _email));

            AutoStateMachineSaga saga = _repository.ShouldContainSaga(_transactionId, 8.Seconds());

            saga.CurrentState.ShouldEqual(AutoStateMachineSaga.WaitingForEmailValidation);

            LocalBus.Publish(new UserValidated(_transactionId));


            saga.ShouldBeInState(AutoStateMachineSaga.Completed, 8.Seconds());
        }
    }
}