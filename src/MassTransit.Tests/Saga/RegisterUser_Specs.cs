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
    using System.Diagnostics;
    using Magnum.TestFramework;
    using MassTransit.Saga;
    using MassTransit.Subscriptions;
    using Messages;
    using NUnit.Framework;
    using TestFramework;
    using TextFixtures;

    [TestFixture]
    public class When_a_unknown_user_registers :
        LoopbackLocalAndRemoteTestFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            InMemorySagaRepository<RegisterUserSaga> sagaRepository = SetupSagaRepository<RegisterUserSaga>();

            // this just shows that you can easily respond to the message
            RemoteBus.SubscribeHandler<SendUserVerificationEmail>(
                x =>
                    {
                        RemoteBus.ShouldHaveSubscriptionFor<UserVerificationEmailSent>();
                        RemoteBus.Publish(new UserVerificationEmailSent(x.CorrelationId, x.Email));
                    });

            RemoteBus.SubscribeSaga(sagaRepository);

            LocalBus.ShouldHaveSubscriptionFor<RegisterUser>();
            LocalBus.ShouldHaveSubscriptionFor<UserVerificationEmailSent>();
            LocalBus.ShouldHaveSubscriptionFor<UserValidated>();
        }

        [Test]
        public void The_user_should_be_pending()
        {
            Stopwatch timer = Stopwatch.StartNew();

            var controller = new RegisterUserController(LocalBus);
            using (IDisposableConnectHandle unsubscribe = LocalBus.SubscribeInstance(controller).Disposable())
            {
                RemoteBus.ShouldHaveSubscriptionFor<UserRegistrationPending>();
                RemoteBus.ShouldHaveSubscriptionFor<UserRegistrationComplete>();

                bool complete = controller.RegisterUser("username", "password", "Display Name", "user@domain.com");

                complete.ShouldBeTrue("The user should be pending");

                timer.Stop();
                Debug.WriteLine(string.Format("Time to handle message: {0}ms", timer.ElapsedMilliseconds));

                complete = controller.ValidateUser();

                complete.ShouldBeTrue("The user should be complete");
            }
        }
    }
}