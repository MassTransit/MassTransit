namespace MassTransit.TestFramework
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using MassTransit.Logging;
    using Mediator;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using Testing;
    using Util;


    public class MediatorTestFixture :
        AsyncTestFixture
    {
        static readonly bool _enableLog = !bool.TryParse(Environment.GetEnvironmentVariable("CI"), out var isBuildServer) || !isBuildServer;
        protected static readonly TestOutputLoggerFactory LoggerFactory;
        TestExecutionContext _fixtureContext;

        static MediatorTestFixture()
        {
            LoggerFactory = new TestOutputLoggerFactory(_enableLog);
        }

        public MediatorTestFixture()
            : this(new MediatorTestHarness())
        {
        }

        public MediatorTestFixture(MediatorTestHarness harness)
            : base(harness)
        {
            MediatorTestHarness = harness;
            MediatorTestHarness.OnConfigureMediator += ConfigureMediator;
        }

        protected MediatorTestHarness MediatorTestHarness { get; }

        protected IMediator Mediator => MediatorTestHarness.Mediator;

        [SetUp]
        public Task SetupInMemoryTest()
        {
            return Task.CompletedTask;
        }

        [TearDown]
        public Task TearDownInMemoryTest()
        {
            return Task.CompletedTask;
        }

        protected IRequestClient<TRequest> CreateRequestClient<TRequest>()
            where TRequest : class
        {
            return MediatorTestHarness.CreateRequestClient<TRequest>();
        }

        [OneTimeSetUp]
        public Task SetupMediatorTestFixture()
        {
            _fixtureContext = TestExecutionContext.CurrentContext;

            LoggerFactory.Current = _fixtureContext;

            return MediatorTestHarness.Start();
        }

        [OneTimeTearDown]
        public async Task TearDownMediatorTestFixture()
        {
            LoggerFactory.Current = _fixtureContext;

            MediatorTestHarness.Dispose();
        }

        protected virtual void ConfigureMediator(IMediatorConfigurator mediatorConfigurator)
        {
            if (_enableLog)
                LogContext.ConfigureCurrentLogContext(LoggerFactory);

            LoggerFactory.Current = default;
        }
    }
}
