namespace MassTransit.Containers.Tests.Common_Tests
{
    using System;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    public abstract class Courier_ExecuteActivity :
        InMemoryTestFixture
    {
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Uri _executeAddress;
        Guid _trackingNumber;

        protected abstract IBusRegistrationContext Registration { get; }

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

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("execute_testactivity", endpointConfigurator =>
            {
                endpointConfigurator.ConfigureExecuteActivity(Registration, typeof(SetVariableActivity));

                _executeAddress = endpointConfigurator.InputAddress;
            });
        }
    }


    public abstract class Courier_ExecuteActivity_Endpoint :
        InMemoryTestFixture
    {
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Guid _trackingNumber;

        protected abstract IBusRegistrationContext Registration { get; }

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

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(Registration);
        }
    }


    public abstract class Courier_Activity :
        InMemoryTestFixture
    {
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Uri _executeAddress;
        Guid _trackingNumber;

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_register_and_execute_the_activity()
        {
            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            builder.AddActivity("TestActivity", _executeAddress, new {Value = "Hello"});

            await Bus.Execute(builder.Build());

            await _completed;
            await _activityCompleted;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("execute_testactivity", endpointConfigurator =>
            {
                configurator.ReceiveEndpoint("compensate_testactivity", compensateConfigurator =>
                {
                    endpointConfigurator.ConfigureActivity(compensateConfigurator, Registration, typeof(TestActivity));
                });

                _executeAddress = endpointConfigurator.InputAddress;
            });
        }
    }


    public abstract class Common_Activity_Filter :
        InMemoryTestFixture
    {
        protected readonly TaskCompletionSource<(TestActivity, MyId)> ActivityTaskCompletionSource;
        protected readonly TaskCompletionSource<MyId> ExecuteTaskCompletionSource;
        Uri _executeAddress;

        protected Common_Activity_Filter()
        {
            ActivityTaskCompletionSource = GetTask<(TestActivity, MyId)>();
            ExecuteTaskCompletionSource = GetTask<MyId>();
        }

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_use_scope()
        {
            var trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            builder.AddActivity("TestActivity", _executeAddress, new {Value = "Hello"});

            await Bus.Execute(builder.Build());

            var result = await ExecuteTaskCompletionSource.Task;
            Assert.IsNotNull(result);

            var activityResult = await ActivityTaskCompletionSource.Task;
            Assert.IsNotNull(activityResult);

            Assert.That(result, Is.EqualTo(activityResult.Item2));

        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("execute_testactivity", endpointConfigurator =>
            {
                configurator.ReceiveEndpoint("compensate_testactivity", compensateConfigurator =>
                {
                    endpointConfigurator.ConfigureActivity(compensateConfigurator, Registration, typeof(TestActivity));
                });

                _executeAddress = endpointConfigurator.InputAddress;
            });
            ConfigureFilter(configurator);
        }

        protected abstract void ConfigureFilter(IConsumePipeConfigurator configurator);

        protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddActivity<TestActivity, TestArguments, TestLog>();
            configurator.AddBus(provider => BusControl);
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

                return context.CompletedWithVariables<TestLog>(new {OriginalValue = context.Arguments.Value}, new
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


        protected class ScopedFilter<T> :
            IFilter<ExecuteContext<T>>
            where T : class
        {
            readonly MyId _myId;
            readonly TaskCompletionSource<MyId> _taskCompletionSource;

            public ScopedFilter(TaskCompletionSource<MyId> taskCompletionSource, MyId myId)
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
    }


    public abstract class Courier_Activity_Endpoint :
        InMemoryTestFixture
    {
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Guid _trackingNumber;

        protected abstract IBusRegistrationContext Registration { get; }

        [Test]
        public async Task Should_register_and_execute_the_activity()
        {
            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            builder.AddActivity("TestActivity", new Uri("loopback://localhost/custom-testactivity-execute"), new {Value = "Hello"});

            await Bus.Execute(builder.Build());

            await _completed;
            await _activityCompleted;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(Registration);
        }
    }
}
