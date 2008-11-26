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
namespace MassTransit.Tests.Saga.RegisterUser
{
    using System;
    using System.Threading;
    using Messages;

    public class RegisterUserController :
        Consumes<UserRegistrationPending>.For<Guid>,
        Consumes<UserRegistrationComplete>.For<Guid>
    {
        private readonly IServiceBus _bus;
        private readonly ManualResetEvent _completed = new ManualResetEvent(false);
        private readonly Guid _correlationId;
        private readonly ManualResetEvent _pending = new ManualResetEvent(false);

        public RegisterUserController(IServiceBus bus)
        {
            _bus = bus;

            _correlationId = Guid.NewGuid();
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

        public void Consume(UserRegistrationPending message)
        {
            _pending.Set();
        }

        public void Consume(UserRegistrationComplete message)
        {
            _completed.Set();
        }

        public bool RegisterUser(string username, string password, string displayName, string email)
        {
            using(Flow.Subscription(_bus, this))
            {
                RegisterUser message = new RegisterUser(_correlationId, "username", "password", "Display Name", "user@domain.com");
                _bus.Publish(message);

                WaitHandle[] handles = new WaitHandle[] {_completed, _pending};

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

        public bool ValidateUser()
        {
            using (Flow.Subscription(_bus, this))
            {
                _bus.Publish(new UserValidated(CorrelationId));

                WaitHandle[] handles = new WaitHandle[] { _completed };

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

    public static class Flow
    {
        public static FlowSubscription<TComponent> Subscription<TComponent>(IServiceBus bus, TComponent subscriber) where TComponent : class
        {
            return new FlowSubscription<TComponent>(bus, subscriber);
        }
    }

    public class FlowSubscription<TComponent> :
        IDisposable 
        where TComponent : class
    {
        private readonly IServiceBus _bus;
        private readonly TComponent _subscriber;

        public FlowSubscription(IServiceBus bus, TComponent subscriber)
        {
            _bus = bus;
            _subscriber = subscriber;

            _bus.Subscribe(_subscriber);
        }

        public void Dispose()
        {
            _bus.Unsubscribe(_subscriber);
        }
    }
}