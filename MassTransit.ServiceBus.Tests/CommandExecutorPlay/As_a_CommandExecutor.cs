namespace MassTransit.ServiceBus.Tests.CommandExecutorPlay
{
    using NUnit.Framework;

    [TestFixture]
    public class As_a_CommandExecutor
    {
        [Test]
        public void I_Should_Execute_the_Command()
        {
            SynchronousCommandExecutor exe = new SynchronousCommandExecutor();
            TestCommand cmd = new TestCommand();
            exe.ExecuteCommand(cmd);
            Assert.IsTrue(cmd.WasCalled);
        }

        [Test]
        public void I_Should_Exectue_the_Command_And_Callback()
        {
            SynchronousCommandExecutor exe = new SynchronousCommandExecutor();
            TestCommand cmd = new TestCommand();
            TestCommand callback = new TestCommand();
            exe.ExecuteCommandWithCallback(cmd, callback);
            Assert.IsTrue(cmd.WasCalled);
            Assert.IsTrue(callback.WasCalled);
        }
    }




    public interface ICommand
    {
        void Execute();
    }

    public interface ICommandExecutor
    {
        void ExecuteCommandWithCallback(ICommand command, ICommand callback);
        void ExecuteCommand(ICommand command);
    }

    public class TestCommand : ICommand
    {
        public bool WasCalled = false;

        public void Execute()
        {
            WasCalled = true;
        }
    }
    public class SynchronousCommandExecutor : ICommandExecutor
    {
        public void Stop()
        {
            // no-op;
        }

        public void ExecuteCommandWithCallback(ICommand command, ICommand callback)
        {
            command.Execute();
            callback.Execute();
        }

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
        }

        public void Start()
        {
            // no-op;
        }
    }
}