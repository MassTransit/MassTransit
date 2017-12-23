// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using Messages;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class When_a_unknown_user_registers :
        InMemoryTestFixture
    {
        [Test]
        public async Task The_user_should_be_pending()
        {
            var timer = Stopwatch.StartNew();

            var controller = new RegisterUserController(Bus);
            HostReceiveEndpointHandle connectReceiveEndpoint = null;
            try
            {
                connectReceiveEndpoint = Host.ConnectReceiveEndpoint(NewId.NextGuid().ToString(), x => x.Instance(controller));
                await connectReceiveEndpoint.Ready;

                bool complete = controller.RegisterUser("username", "password", "Display Name", "user@domain.com");

                complete.ShouldBe(true); //("The user should be pending");

                timer.Stop();
                Debug.WriteLine("Time to handle message: {0}ms", timer.ElapsedMilliseconds);

                complete = controller.ValidateUser();

                complete.ShouldBe(true); //("The user should be complete");
            }
            finally
            {
                if (connectReceiveEndpoint != null)
                    await connectReceiveEndpoint.StopAsync();
            }
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            var sagaRepository = new InMemorySagaRepository<RegisterUserSaga>();
            configurator.Saga(sagaRepository);

            configurator.Handler<SendUserVerificationEmail>(async x =>
            {
                await Bus.Publish(new UserVerificationEmailSent(x.Message.CorrelationId, x.Message.Email));
            });
        }
    }
}