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

        [OneTimeSetUp]
        public async Task Setup()
        {
            await InputQueueSendEndpoint.Send(new CommandAndCredentials
            {
                SqlText = "DROP TABLE [Users]",
                Username = "sa",
                Password = "god"
            }, TestCancellationToken);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _commandHandler = Handler<SecureCommand>(configurator, async context =>
            {
                if (context.TryGetMessage(out ConsumeContext<UserCredentials> credentials))
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


        #pragma warning disable NUnit1032
        Task<ConsumeContext<SecureCommand>> _commandHandler;
        readonly TaskCompletionSource<UserCredentials> _credentials = TaskUtil.GetTask<UserCredentials>();
        Task<ConsumeContext<UserCredentials>> _credentialsHandler;
        #pragma warning restore NUnit1032
    }
}
