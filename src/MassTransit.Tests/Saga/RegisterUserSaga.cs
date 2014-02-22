// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Tests.Saga
{
	using System;
	using Magnum.Extensions;
	using MassTransit.Saga;
	using Messages;
	using TestFramework;

	/// <summary>
	/// 
	/// So here is the deal
	/// The saga is a class that contains the state and behavior of the saga
	/// for messages that are saga messages. If the message is not a saga message
	/// it doesn't need any details about the saga in order to proceed.
	/// 
	/// This will allow services that are not saga-specific to participate in a saga
	/// 
	/// So something like an e-mail sender would send an e-mail, publish the mail sent message
	/// and that would trigger the saga to continue according to the result.
	/// 
	/// By doing so, the saga can then return to power after being away for a while.
	/// 
	/// </summary>
	public class RegisterUserSaga :
		InitiatedBy<RegisterUser>,
		Orchestrates<UserVerificationEmailSent>,
		Orchestrates<UserValidated>,
		ISaga
	{
		public string DisplayName { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }

		public string Username { get; set; }

		protected RegisterUserSaga()
		{
		}

		public RegisterUserSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

		public Guid Id
		{
			get { return CorrelationId; }
		}

		public void Consume(RegisterUser message)
		{
			CorrelationId = message.CorrelationId;
			DisplayName = message.DisplayName;
			Username = message.Username;
			Password = message.Password;
			Email = message.Email;

			Bus.Publish(new SendUserVerificationEmail(CorrelationId, Email));
		}

		// The bus that received the message
		public IServiceBus Bus { get; set; }

		public Guid CorrelationId { get; private set; }

		public void Consume(UserValidated message)
		{
			// at this point, the user has clicked the link in the validation e-mail
			// and we can commit the user record to the database as a verified user

			Bus.Publish(new UserRegistrationComplete(CorrelationId));

			Complete();
		}

		public void Consume(UserVerificationEmailSent message)
		{
			// once the verification e-mail has been sent, we allow 24 hours to pass before we 
			// remove this transaction from the registration queue

			if (Email != message.Email)
				throw new ArgumentException("The email address was not properly loaded.");

			Bus.ShouldHaveSubscriptionFor<UserRegistrationPending>();

			Bus.Publish(new UserRegistrationPending(CorrelationId));
		}

		private void Complete()
		{
		}
	}
}