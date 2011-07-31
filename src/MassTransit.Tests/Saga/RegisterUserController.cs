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
namespace MassTransit.Tests.Saga
{
	using System;
	using System.Threading;
	using Magnum.Extensions;
	using Messages;

	public class RegisterUserController :
		Consumes<UserRegistrationPending>.For<Guid>,
		Consumes<UserRegistrationComplete>.For<Guid>
	{
		readonly IServiceBus _bus;
		readonly ManualResetEvent _completed = new ManualResetEvent(false);
		readonly Guid _correlationId;
		readonly ManualResetEvent _pending = new ManualResetEvent(false);

		public RegisterUserController(IServiceBus bus)
		{
			_bus = bus;

			_correlationId = Guid.NewGuid();
		}

		public void Consume(UserRegistrationComplete message)
		{
			_completed.Set();
		}

		public Guid CorrelationId
		{
			get { return _correlationId; }
		}

		public void Consume(UserRegistrationPending message)
		{
			_pending.Set();
		}

		public bool RegisterUser(string username, string password, string displayName, string email)
		{
			var message = new RegisterUser(_correlationId, "username", "password", "Display Name", "user@domain.com");

			bool result = false;

			_bus.PublishRequest(message, x =>
				{
					x.Handle<UserRegistrationPending>(response => { result = false; });
					x.Handle<UserRegistrationComplete>(response => { result = true; });
					x.SetTimeout(10.Seconds());
				});

			return result;
		}

		public bool ValidateUser()
		{
			using (_bus.SubscribeInstance(this).Disposable())
			{
				Thread.Sleep(5.Seconds());

				_bus.Publish(new UserValidated(CorrelationId));

				var handles = new WaitHandle[] {_completed};

				int result = WaitHandle.WaitAny(handles, TimeSpan.FromSeconds(10), true);

				if (result == 0)
				{
					// we have success!
					return true;
				}

				if (result == 1)
				{
					// we are pending, so we need to return false but not fail
					return false;
				}

				throw new ApplicationException("A timeout occurred while registering the user");
			}
		}
	}
}