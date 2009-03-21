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
	using MassTransit.Pipeline.Inspectors;
	using MassTransit.Saga;
	using Messages;
	using NUnit.Framework;
	using TextFixtures;

	[TestFixture]
	public class AutoStateMachine_Specs :
		LoopbackLocalAndRemoteTestFixture
	{
		private ISagaRepository<AutoStateMachineSaga> _repository;

		protected override void EstablishContext()
		{
			base.EstablishContext();

			_repository = SetupSagaRepository<AutoStateMachineSaga>();
			SetupInitiateSagaStateMachineSink<AutoStateMachineSaga, RegisterUser>(RemoteBus, _repository);
			SetupOrchestrateSagaStateMachineSink<AutoStateMachineSaga, UserVerificationEmailSent>(RemoteBus, _repository);
			SetupOrchestrateSagaStateMachineSink<AutoStateMachineSaga, UserValidated>(RemoteBus, _repository);

			// this just shows that you can easily respond to the message
			RemoteBus.Subscribe<SendUserVerificationEmail>(
				x => RemoteBus.Publish(new UserVerificationEmailSent(x.CorrelationId, x.Email)));

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
			RemoteBus.Subscribe<AutoStateMachineSaga>();

			PipelineViewer.Trace(RemoteBus.InboundPipeline);

			PipelineViewer.Trace(RemoteBus.OutboundPipeline);


			Assert.AreEqual(0, _repository.Count());

			RemoteBus.Publish(new RegisterUser(_transactionId, _username, _password, _displayName, _email));

			for (int i = 0; i < 20; i++)
			{
				if (_repository.Count() == 1)
					break;

				Thread.Sleep(100);
			}

			Assert.AreEqual(1, _repository.Count());

			foreach (AutoStateMachineSaga saga in _repository)
			{
				saga.CurrentState.ShouldEqual(AutoStateMachineSaga.WaitingForEmailValidation);
			}

			_repository
				.Where(x => x.CorrelationId == _transactionId)
				.Select(x => x.CurrentState)
				.First()
				.ShouldEqual(AutoStateMachineSaga.WaitingForEmailValidation);

			RemoteBus.Publish(new UserValidated(_transactionId));

			for (int i = 0; i < 20; i++)
			{
				if (_repository.Where(x => x.CorrelationId == _transactionId).Select(x => x.CurrentState).First() == AutoStateMachineSaga.Completed)
					break;

				Thread.Sleep(100);
			}

			_repository
				.Where(x => x.CorrelationId == _transactionId)
				.Select(x => x.CurrentState)
				.First()
				.ShouldEqual(AutoStateMachineSaga.Completed);
		}
	}
}