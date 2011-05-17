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
namespace MassTransit.Tests.Saga.StateMachine
{
	using System;
	using System.Linq;
	using System.Threading;
	using Magnum;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using MassTransit.Pipeline.Inspectors;
	using MassTransit.Saga;
	using Messages;
	using NUnit.Framework;
	using TextFixtures;
	using TestFramework;

	[TestFixture]
	public class AutoStateMachine_Specs :
		LoopbackTestFixture
	{
		private ISagaRepository<AutoStateMachineSaga> _repository;

		protected override void EstablishContext()
		{
			base.EstablishContext();

			_repository = SetupSagaRepository<AutoStateMachineSaga>();

			// this just shows that you can easily respond to the message
			LocalBus.SubscribeHandler<SendUserVerificationEmail>(
				x => LocalBus.Publish(new UserVerificationEmailSent(x.CorrelationId, x.Email)));

			_transactionId = CombGuid.Generate();
			_username = "jblow";
			_password = "password1";
			_email = "jblow@yourdad.com";
			_displayName = "Joe Blow";
		}

		private Guid _transactionId;
		private string _username;
		private string _password;
		private string _email;
		private string _displayName;

		[Test]
		public void A_state_machine_based_saga_should_automatically_wire_up_subscriptions()
		{
			LocalBus.SubscribeSaga<AutoStateMachineSaga>(_repository);

			PipelineViewer.Trace(LocalBus.InboundPipeline);
			PipelineViewer.Trace(LocalBus.OutboundPipeline);

			LocalBus.Publish(new RegisterUser(_transactionId, _username, _password, _displayName, _email));

			var saga = _repository.ShouldContainSaga(_transactionId, 8.Seconds());

			saga.CurrentState.ShouldEqual(AutoStateMachineSaga.WaitingForEmailValidation);

			LocalBus.Publish(new UserValidated(_transactionId));


			saga.ShouldBeInState(AutoStateMachineSaga.Completed, 8.Seconds());
		}
	}
}