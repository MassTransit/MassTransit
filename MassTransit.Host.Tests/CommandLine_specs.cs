namespace MassTransit.Host.Tests
{
    using Configurations;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class CommandLine_specs
    {
        MockRepository mocks;
        readonly string[] installArgs = new[] { "-install" };
        readonly string[] uninstallArgs = new[] { "-uninstall" };
        readonly string[] consoleArgs = new[] { "-console" };
        readonly string[] serviceArgs = new[] { "-service" };
        IInstallationConfiguration configuration;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            configuration = mocks.CreateMock<IInstallationConfiguration>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            configuration = null;
        }

        [Test]
        public void When_it_gets_install_should_install()
        {

            using (mocks.Record())
            {
				Expect.Call(configuration.LifeCycle).Return(null);
				Expect.Call(configuration.ServiceName).Return("MTUnitTest").Repeat.Any();
			}
            using (mocks.Playback())
            {
                Runner.Run(configuration, installArgs);
            }
        }

        [Test]
        public void When_it_gets_uninstall_should_uninstall()
        {
            using (mocks.Record())
            {
            	Expect.Call(configuration.ServiceName).Return("MTUnitTest").Repeat.Any();
            }
            using (mocks.Playback())
            {
                Runner.Run(configuration, uninstallArgs);
            }
        }

        [Test]
        public void Should_run_as_command_line()
        {

        }

        [Test]
        public void Should_run_as_command_line_by_default()
        {

        }

        [Test]
        public void Should_run_as_a_service()
        {

        }

        [Test]
        public void Test_parser()
        {
            Assert.AreEqual("install", Parser.ParseArgs(installArgs).GetActionKey());
            Assert.AreEqual("uninstall", Parser.ParseArgs(uninstallArgs).GetActionKey());
            Assert.AreEqual("console", Parser.ParseArgs(consoleArgs).GetActionKey());
            Assert.AreEqual("service", Parser.ParseArgs(serviceArgs).GetActionKey());

            Assert.AreEqual("console", Parser.ParseArgs(null).GetActionKey());
            Assert.AreEqual("console", Parser.ParseArgs(new string[] { "" }).GetActionKey());
            Assert.AreEqual("console", Parser.ParseArgs(new string[0]).GetActionKey());
        }


    }
}