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
namespace MassTransit.Tests
{
    using System.Linq;
    using BusConfigurators;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using TextFixtures;
    using MassTransit.Testing;

    [TestFixture]
    public class Putting_two_bodies_into_one_message :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Should_be_possible()
        {
            RemoteBus.HasSubscription<SecureCommand>(8.Seconds()).Any().ShouldBeTrue();

            RemoteBus.Publish(new CommandAndCredentials
                {
                    SqlText = "DROP TABLE [Users]",
                    Username = "sa",
                    Password = "god",
                });

            CommandHandler.CredentialsReceived.IsAvailable(8.Seconds()).ShouldBeTrue();
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.Subscribe(x => { x.Consumer<CommandHandler>(); });
        }


        public class CommandHandler :
            Consumes<SecureCommand>.Context
        {
            public static FutureMessage<UserCredentials> CredentialsReceived = new FutureMessage<UserCredentials>();

            public void Consume(IConsumeContext<SecureCommand> context)
            {
                IConsumeContext<UserCredentials> credentials;
                if (context.TryGetContext(out credentials))
                {
                    CredentialsReceived.Set(credentials.Message);
                }
            }
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

    [TestFixture]
    public class Wrapping_a_message_in_a_generic_wrapper :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Should_be_possible()
        {
            LocalBus.HasSubscription<BusinessCommand>(8.Seconds()).Any().ShouldBeTrue();
            RemoteBus.HasSubscription<SecureCommand<BusinessCommand>>(8.Seconds()).Any().ShouldBeTrue();

            RemoteBus.Publish(new BusinessCommandImpl
                {
                    SqlText = "DROP TABLE [Users]",
                });

            CommandHandler.CommandReceived.IsAvailable(8.Seconds()).ShouldBeTrue();
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.Subscribe(x => { x.Consumer<CommandHandler>(); });
        }

        protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.Subscribe(x => x.Consumer<CommandSecurityMaker<BusinessCommand>>());
        }


        public class CommandHandler :
            Consumes<SecureCommand<BusinessCommand>>.Context
        {
            public static FutureMessage<UserCredentials> CommandReceived = new FutureMessage<UserCredentials>();

            public void Consume(IConsumeContext<SecureCommand<BusinessCommand>> context)
            {
                CommandReceived.Set(context.Message.Credentials);
            }
        }


        class SecureCommandImpl<T> :
            SecureCommand<T>
            where T : class
        {
            public T Command { get;  set; }
            public UserCredentials Credentials { get;  set; }
        }

        public interface SecureCommand<T>
            where T : class
        {
            T Command { get; }
            UserCredentials Credentials { get; }
        }

        public interface BusinessCommand
        {
            string SqlText { get; }
        }

        class BusinessCommandImpl : 
            BusinessCommand
        {
            public string SqlText { get; set; }
        }

        public interface UserCredentials
        {
            string Username { get; }
            string Password { get; }
        }

        class UserCredentialsImpl : UserCredentials
        {
            public string Username { get;  set; }
            public string Password { get;  set; }
        }

        public class CommandSecurityMaker<T> :
            Consumes<T>.Context
            where T : class
        {
            public void Consume(IConsumeContext<T> message)
            {
                var output = new SecureCommandImpl<T>()
                    {
                        Command = message.Message,
                        Credentials = new UserCredentialsImpl {Username = "sa", Password = "god"},
                    };

                message.Bus.Publish(output, x => x.ForwardUsingOriginalContext(message));
            }
        }
    }
}