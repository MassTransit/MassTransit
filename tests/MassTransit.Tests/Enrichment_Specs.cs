namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using Util;


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
        readonly TaskCompletionSource<UserCredentials> _credentials = TaskUtil.GetTask<UserCredentials>();
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
