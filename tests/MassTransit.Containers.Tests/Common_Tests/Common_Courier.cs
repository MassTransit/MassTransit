namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    public class Courier_ExecuteActivity<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_register_and_execute_the_activity()
        {
            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            builder.AddActivity("SetVariableActivity", _executeAddress, new
            {
                Key = "Hello",
                Value = "Hello"
            });

            await Bus.Execute(builder.Build());

            await _completed;
            await _activityCompleted;
        }

        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Uri _executeAddress;
        Guid _trackingNumber;

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddExecuteActivity<SetVariableActivity, SetVariableArguments>();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("execute_testactivity", endpointConfigurator =>
            {
                endpointConfigurator.ConfigureExecuteActivity(BusRegistrationContext, typeof(SetVariableActivity));

                _executeAddress = endpointConfigurator.InputAddress;
            });
        }
    }


    public class Courier_ExecuteActivity_Endpoint<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_register_and_execute_the_activity()
        {
            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            builder.AddActivity("SetVariableActivity", new Uri("loopback://localhost/custom-setvariable-execute"), new
            {
                Key = "Hello",
                Value = "Hello"
            });

            await Bus.Execute(builder.Build());

            await _completed;
            await _activityCompleted;
        }

        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Guid _trackingNumber;

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddExecuteActivity<SetVariableActivity, SetVariableArguments>()
                .Endpoint(e => e.Name = "custom-setvariable-execute");
        }
    }


    public class Courier_Activity<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_register_and_execute_the_activity()
        {
            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            builder.AddActivity("TestActivity", _executeAddress, new { Value = "Hello" });

            await Bus.Execute(builder.Build());

            await _completed;
            await _activityCompleted;
        }

        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Uri _executeAddress;
        Guid _trackingNumber;

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddActivity<TestActivity, TestArguments, TestLog>();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("execute_testactivity", endpointConfigurator =>
            {
                configurator.ReceiveEndpoint("compensate_testactivity", compensateConfigurator =>
                {
                    endpointConfigurator.ConfigureActivity(compensateConfigurator, BusRegistrationContext, typeof(TestActivity));
                });

                _executeAddress = endpointConfigurator.InputAddress;
            });
        }
    }


    public class Common_Activity_Filter<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_use_scope()
        {
            var trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            builder.AddActivity("TestActivity", _executeAddress, new { Value = "Hello" });

            await Bus.Execute(builder.Build());

            var result = await _executeTaskCompletionSource.Task;
            Assert.IsNotNull(result);

            var activityResult = await _activityTaskCompletionSource.Task;
            Assert.IsNotNull(activityResult);

            Assert.That(result, Is.EqualTo(activityResult.Item2));
        }

        readonly TaskCompletionSource<(TestActivity, MyId)> _activityTaskCompletionSource;
        readonly TaskCompletionSource<MyId> _executeTaskCompletionSource;
        Uri _executeAddress;

        public Common_Activity_Filter()
        {
            _activityTaskCompletionSource = GetTask<(TestActivity, MyId)>();
            _executeTaskCompletionSource = GetTask<MyId>();
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection collection)
        {
            return collection.AddScoped(_ => new MyId(Guid.NewGuid()))
                .AddScoped(typeof(TestActivityScopedFilter<>))
                .AddSingleton(_activityTaskCompletionSource)
                .AddSingleton(_executeTaskCompletionSource);
        }

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddActivity<TestActivity, TestArguments, TestLog>();
        }

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddActivity<TestActivity, TestArguments, TestLog>();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("execute_testactivity", endpointConfigurator =>
            {
                configurator.ReceiveEndpoint("compensate_testactivity", compensateConfigurator =>
                {
                    endpointConfigurator.ConfigureActivity(compensateConfigurator, BusRegistrationContext, typeof(TestActivity));
                });

                _executeAddress = endpointConfigurator.InputAddress;
            });

            configurator.UseExecuteActivityFilter(typeof(TestActivityScopedFilter<>), BusRegistrationContext);
        }


        public class TestActivity :
            IActivity<TestArguments, TestLog>
        {
            readonly MyId _myId;
            readonly TaskCompletionSource<(TestActivity, MyId)> _taskCompletionSource;

            public TestActivity(TaskCompletionSource<(TestActivity, MyId)> taskCompletionSource, MyId myId)
            {
                _taskCompletionSource = taskCompletionSource;
                _myId = myId;
            }

            public async Task<ExecutionResult> Execute(ExecuteContext<TestArguments> context)
            {
                if (context.Arguments.Value == null)
                    throw new ArgumentNullException(nameof(context.Arguments.Value));

                _taskCompletionSource.SetResult((this, _myId));

                return context.CompletedWithVariables<TestLog>(new { OriginalValue = context.Arguments.Value }, new
                {
                    Value = "Hello, World!",
                    NullValue = (string)null
                });
            }

            public async Task<CompensationResult> Compensate(CompensateContext<TestLog> context)
            {
                Console.WriteLine("TestActivity: Compensate original value: {0}", context.Log.OriginalValue);

                return context.Compensated();
            }
        }
    }


    public class TestActivityScopedFilter<T> :
        IFilter<ExecuteContext<T>>
        where T : class
    {
        readonly MyId _myId;
        readonly TaskCompletionSource<MyId> _taskCompletionSource;

        public TestActivityScopedFilter(TaskCompletionSource<MyId> taskCompletionSource, MyId myId)
        {
            _taskCompletionSource = taskCompletionSource;
            _myId = myId;
        }

        public Task Send(ExecuteContext<T> context, IPipe<ExecuteContext<T>> next)
        {
            _taskCompletionSource.TrySetResult(_myId);
            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }


    public class Courier_Activity_Endpoint<TContainer> :
        CommonContainerTestFixture<TContainer>
        where TContainer : ITestFixtureContainerFactory, new()
    {
        [Test]
        public async Task Should_register_and_execute_the_activity()
        {
            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            builder.AddActivity("TestActivity", new Uri("loopback://localhost/custom-testactivity-execute"), new { Value = "Hello" });

            await Bus.Execute(builder.Build());

            await _completed;
            await _activityCompleted;
        }

        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Guid _trackingNumber;

        protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
            configurator.AddActivity<TestActivity, TestArguments, TestLog>()
                .Endpoints(e => e.Name = "custom-testactivity-execute", e => e.Name = "custom-testactivity-compensate");
        }
    }
}
