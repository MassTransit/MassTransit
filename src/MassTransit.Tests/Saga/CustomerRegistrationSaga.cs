// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Magnum.StateMachine;
    using MassTransit.Saga;

    public class CustomerRegistrationSaga :
        SagaStateMachine<CustomerRegistrationSaga>,
        ISaga
    {
        static CustomerRegistrationSaga()
        {
            Define(() =>
                {
                    Correlate(Initialised).By((saga, message) => saga.SystemId == message.SystemId);
                    Correlate(SettingsCreated).By((saga, message) => saga.SystemId == message.SystemId);
                    Correlate(UserCreated).By((saga, message) => saga.SystemId == message.SystemId);
                    Correlate(SystemActivated).By((saga, message) => saga.SystemId == message.SystemId);

                    Initially(
                        When(Initialised)
                            .Then((saga, message) => saga.SystemId = message.SystemId)
                            .Then((saga, message) => saga.Bus.Publish(new CreateSettingsCommand()))
                            .Then((saga, message) => saga.Bus.Publish(new CreateUserCommand()))
                            .TransitionTo(WaitingForCustomerAndSettings));

                    Combine(SettingsCreated, UserCreated)
                        .Into(SettingsAndUserCreated, saga => saga.ReadyFlags);

                    During(
                        WaitingForCustomerAndSettings,
                        When(SettingsAndUserCreated)
                            .Then((saga, message) => saga.Bus.Publish(new ActivateSystemCommand()))
                            .TransitionTo(WaitingForSystemActivation));

                    During(
                        WaitingForSystemActivation,
                        When(SystemActivated)
                            .Complete());
                });
        }

        public CustomerRegistrationSaga(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        protected CustomerRegistrationSaga()
        {
        }

        public Guid SystemId { get; set; }

        public int ReadyFlags { get; private set; }

        public static State Initial { get; set; }
        public static State WaitingForCustomerAndSettings { get; set; }
        public static State WaitingForSystemActivation { get; set; }
        public static State Completed { get; set; }

        public static Event<ClientSystemCreatedEvent> Initialised { get; set; }
        public static Event<SettingsCreatedEvent> SettingsCreated { get; set; }
        public static Event<UserCreatedEvent> UserCreated { get; set; }
        public static Event<SystemActivatedEvent> SystemActivated { get; set; }
        public static Event SettingsAndUserCreated { get; set; }

        #region ISaga Members

        public Guid CorrelationId { get; set; }
        public IServiceBus Bus { get; set; }

        #endregion
    }

    public class ClientSystemCreatedEvent
        //: CorrelatedBy<Guid>
    {
        public Guid SystemId { get; set; }
        //public Guid CorrelationId { get; set; }
    }

    public class UserCreatedEvent
        //: CorrelatedBy<Guid>
    {
        public Guid SystemId { get; set; }
        //public Guid CorrelationId { get; set; }
    }

    public class SettingsCreatedEvent
        //: CorrelatedBy<Guid>
    {
        public Guid SystemId { get; set; }
        //public Guid CorrelationId { get; set; }
    }

    public class SystemActivatedEvent
        //: CorrelatedBy<Guid>
    {
        public Guid SystemId { get; set; }
        //public Guid CorrelationId { get; set; }
    }

    public class ActivateSystemCommand
    {
    }

    public class CreateUserCommand
    {
    }

    public class CreateSettingsCommand
    {
    }
}