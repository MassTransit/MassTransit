namespace MassTransit.TestFramework.Logging
{
    using Microsoft.Extensions.Logging;


    public class TestOutputLoggerFactory :
        ILoggerFactory
    {
        readonly bool _enabled;

        public TestOutputLoggerFactory(bool enabled)
        {
            _enabled = enabled;
        }

        public ILogger CreateLogger(string name)
        {
            return new TestOutputLogger(_enabled);
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }

        public void Dispose()
        {
        }
    }
}
