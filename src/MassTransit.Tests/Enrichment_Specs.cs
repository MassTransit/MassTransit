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
    using TestFramework;


    [TestFixture]
    public class Sending_two_interfaces_in_one_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_credentials_alone()
        {
            await _credentialsHandler;
        }

        [Test]
        public async Task Should_receive_the_secure_command()
        {
            await _commandHandler;
        }

        [Test]
        public async Task Should_receive_the_user_credentials()
        {
            await _credentials.Task;
        }

        Task<ConsumeContext<SecureCommand>> _commandHandler;
        readonly TaskCompletionSource<UserCredentials> _credentials = new TaskCompletionSource<UserCredentials>();
        Task<ConsumeContext<UserCredentials>> _credentialsHandler;

        [OneTimeSetUp]
        public void Setup()
        {
            InputQueueSendEndpoint.Send(new CommandAndCredentials
            {
                SqlText = "DROP TABLE [Users]",
                Username = "sa",
                Password = "god",
            }).Wait(TestCancellationToken);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _commandHandler = Handler<SecureCommand>(configurator, async context =>
            {
                ConsumeContext<UserCredentials> credentials;
                if (context.TryGetMessage(out credentials))
                    _credentials.SetResult(credentials.Message);
            });

            _credentialsHandler = Handled<UserCredentials>(configurator);
        }


        class CommandAndCredentials :
            SecureCommand,
            UserCredentials
        {
            public string SqlText { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }


        public interface SecureCommand
        {
            string SqlText { get; }
        }


        public interface UserCredentials
        {
            string Username { get; }
            string Password { get; }
        }
    }
}