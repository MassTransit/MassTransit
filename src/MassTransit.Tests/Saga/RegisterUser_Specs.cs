// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using MassTransit.Saga;
    using Messages;
    using NUnit.Framework;
    using Shouldly;
    using TextFixtures;


    [TestFixture]
    public class When_a_unknown_user_registers :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void The_user_should_be_pending()
        {
            Stopwatch timer = Stopwatch.StartNew();

            var controller = new RegisterUserController(LocalBus);
            using (ConnectHandle unsubscribe = LocalBus.ConnectInstance(controller))
            {
                bool complete = controller.RegisterUser("username", "password", "Display Name", "user@domain.com");

                complete.ShouldBe(true); //("The user should be pending");

                timer.Stop();
                Debug.WriteLine("Time to handle message: {0}ms", timer.ElapsedMilliseconds);

                complete = controller.ValidateUser();

                complete.ShouldBe(true); //("The user should be complete");
            }
        }

        protected void EstablishContext()
        {
            InMemorySagaRepository<RegisterUserSaga> sagaRepository = SetupSagaRepository<RegisterUserSaga>();

            // this just shows that you can easily respond to the message
            RemoteBus.ConnectHandler<SendUserVerificationEmail>(
                async x =>
                {
                    await RemoteBus.Publish(new UserVerificationEmailSent(x.Message.CorrelationId, x.Message.Email));
                });

            RemoteBus.ConnectSaga(sagaRepository);
        }
    }
}