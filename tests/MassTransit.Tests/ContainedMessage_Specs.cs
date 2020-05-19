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
namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class Wrapping_a_message_in_a_generic_wrapper :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_outer_command()
        {
            await _commandHandler;
        }

        [Test]
        public async Task Should_receive_the_secure_command()
        {
            var context = await _secureCommandHandler;

            context.Message.Credentials.ShouldNotBe(null);

            context.Message.Credentials.Username.ShouldBe("sa");
        }

        Task<ConsumeContext<SecureCommand<ExecuteSql>>> _secureCommandHandler;
        Task<ConsumeContext<ExecuteSql>> _commandHandler;

        [OneTimeSetUp]
        public void Setup()
        {
            InputQueueSendEndpoint.Send(new ExecuteSqlCommand
            {
                SqlText = "DROP TABLE [Users]",
            }).Wait(TestCancellationToken);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _secureCommandHandler = Handled<SecureCommand<ExecuteSql>>(configurator);

            _commandHandler = Handler<ExecuteSql>(configurator, async context =>
            {
                var output = new SecureCommandMessage<ExecuteSql>
                {
                    Command = context.Message,
                    Credentials = new UserCredentialsImpl {Username = "sa", Password = "n0_p4ssw0rd"},
                };

                // TODO create ForwardContext pipe that copies the good bits of send context from the original
                await InputQueueSendEndpoint.Send(output, context.CancellationToken);
            });
        }


        class SecureCommandMessage<T> :
            SecureCommand<T>
            where T : class
        {
            public T Command { get; set; }
            public UserCredentials Credentials { get; set; }
        }


        public interface SecureCommand<out T>
            where T : class
        {
            T Command { get; }
            UserCredentials Credentials { get; }
        }


        public interface ExecuteSql
        {
            string SqlText { get; }
        }


        class ExecuteSqlCommand :
            ExecuteSql
        {
            public string SqlText { get; set; }
        }


        public interface UserCredentials
        {
            string Username { get; }
            string Password { get; }
        }


        class UserCredentialsImpl :
            UserCredentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}