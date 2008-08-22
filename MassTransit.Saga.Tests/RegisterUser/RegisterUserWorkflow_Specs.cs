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
namespace MassTransit.Workflow.Tests.RegisterUser
{
    using Castle.Windsor;
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Tests;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using ServiceBus;
    using WindsorIntegration;

    // This test is to simulate an unknown user registering for access to a system
    // the user is not on the pre-approved list, so the registration will initially be pending
    // until the verification e-mail link is clicked. Therefore, the pending registration will
    // later be clicked and continued to complete the workflow and send the registration complete
    // message

    [TestFixture]
    public class When_a_unknown_user_registers :
        Specification
    {
        private IServiceBus _bus;
        private ISagaRepository<RegisterUserSaga> _sagaRepository;

        protected override void Before_each()
        {
            _sagaRepository = DynamicMock<ISagaRepository<RegisterUserSaga>>();

            IWindsorContainer container = new DefaultMassTransitContainer(@"RegisterUser\TestContainer.xml");

            _bus = container.Resolve<IServiceBus>();

            // this just shows that you can easily respond to the message
            _bus.Subscribe<SendUserVerificationEmail>(x => _bus.Publish(new UserVerificationEmailSent(x.Message.CorrelationId)));

            _bus.AddComponent<RegisterUserSaga>();

            container.Kernel.AddComponentInstance<ISagaRepository<RegisterUserSaga>>(_sagaRepository);

            // this will create a component dispatcher for sagas called SagaConsumerDispatcher<> 
            // and bind it to each interface
        }

        [Test]
        public void The_user_should_be_pending()
        {
            RegisterUserController controller = new RegisterUserController(_bus);

            bool complete = controller.RegisterUser("username", "password", "Display Name", "user@domain.com");

            Assert.That(complete, Is.False, "The user should be pending");
        }
    }
}