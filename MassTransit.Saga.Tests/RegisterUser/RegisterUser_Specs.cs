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
namespace MassTransit.Saga.Tests.RegisterUser
{
    using Castle.Core;
    using Messages;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class When_a_unknown_user_registers :
        SagaTestContext
    {
        protected override void Before_each()
        {
            Container.Kernel.AddComponent<ISagaRepository<RegisterUserSaga>>(typeof (RegisterUserSagaRepository), LifestyleType.Transient);

            // this just shows that you can easily respond to the message
            RemoteBus.Subscribe<SendUserVerificationEmail>(x => RemoteBus.Publish(new UserVerificationEmailSent(x.Message.CorrelationId)));

            RemoteBus.AddComponent<RegisterUserSaga>();
        }

        [Test]
        public void The_user_should_be_pending()
        {
            RegisterUserController controller = new RegisterUserController(LocalBus);

            bool complete = controller.RegisterUser("username", "password", "Display Name", "user@domain.com");

            Assert.That(complete, Is.False, "The user should be pending");
        }
    }
}