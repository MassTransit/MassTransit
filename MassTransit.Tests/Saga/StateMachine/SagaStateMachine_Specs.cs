namespace MassTransit.Tests.Saga.StateMachine
{
	using System;
	using Messages;
	using NUnit.Framework;
	using Util;

	[TestFixture]
	public class SagaStateMachine_Specs
	{
		private Guid _transactionId;
		private string _username;
		private string _password;
		private string _email;
		private string _displayName;

		[SetUp]
		public void Setup()
		{
			_transactionId = CombGuid.NewCombGuid();
			_username = "jblow";
			_password = "password1";
			_email = "jblow@yourdad.com";
			_displayName = "Joe Blow";
		}

		[Test]
		public void The_saga_state_machine_should_add_value_for_sagas()
		{
			RegisterUserStateMachine workflow = new RegisterUserStateMachine();

			Assert.AreEqual(RegisterUserStateMachine.Initial, workflow.Current);

			workflow.Consume(new RegisterUser(_transactionId, _username, _password, _displayName, _email));

			Assert.AreEqual(RegisterUserStateMachine.WaitingForEmailValidation, workflow.Current);
		}

		[Test]
		public void The_good_times_should_roll()
		{
			RegisterUserStateMachine workflow = new RegisterUserStateMachine();

			Assert.AreEqual(RegisterUserStateMachine.Initial, workflow.Current);

			workflow.Consume(new RegisterUser(_transactionId, _username, _password, _displayName, _email));

			Assert.AreEqual(RegisterUserStateMachine.WaitingForEmailValidation, workflow.Current);

			workflow.Consume(new UserValidated(_transactionId));

			Assert.AreEqual(RegisterUserStateMachine.Completed, workflow.Current);
		}
	}
}