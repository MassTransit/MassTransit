namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
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
            ConsumeContext<SecureCommand<ExecuteSql>> context = await _secureCommandHandler;

            Assert.That(context.Message.Credentials, Is.Not.Null);

            Assert.That(context.Message.Credentials.Username, Is.EqualTo("sa"));
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<SecureCommand<ExecuteSql>>> _secureCommandHandler;
        Task<ConsumeContext<ExecuteSql>> _commandHandler;
        #pragma warning restore NUnit1032

        [OneTimeSetUp]
        public void Setup()
        {
            InputQueueSendEndpoint.Send(new ExecuteSqlCommand {SqlText = "DROP TABLE [Users]"}).Wait(TestCancellationToken);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _secureCommandHandler = Handled<SecureCommand<ExecuteSql>>(configurator);

            _commandHandler = Handler<ExecuteSql>(configurator, async context =>
            {
                var output = new SecureCommandMessage<ExecuteSql>
                {
                    Command = context.Message,
                    Credentials = new UserCredentialsImpl
                    {
                        Username = "sa",
                        Password = "n0_p4ssw0rd"
                    }
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
