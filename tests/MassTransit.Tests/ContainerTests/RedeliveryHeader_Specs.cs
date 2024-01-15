namespace MassTransit.Tests.ContainerTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier.Contracts;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using RedeliverySubjects;


    [TestFixture]
    public class When_an_activity_is_redelivered
    {
        [Test]
        [Explicit]
        public async Task Should_not_copy_redelivery_header_to_next_activity()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetSnakeCaseEndpointNameFormatter();
                    x.AddSingleton<IEndpointAddressProvider, InMemoryEndpointAddressProvider>();

                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10), testTimeout: TimeSpan.FromMinutes(1));

                    x.AddConsumersFromNamespaceContaining<CommandA>();
                    x.AddActivitiesFromNamespaceContaining<CommandA>();

                    x.AddConfigureEndpointsCallback((context, name, cfg) =>
                    {
                        cfg.UseDelayedRedelivery(r => r.Immediate(3));
                        cfg.UseMessageRetry(r =>
                        {
                            r.Immediate(5);
                            r.Ignore(typeof(BusinessException));
                        });
                        cfg.UseInMemoryOutbox(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = await provider.StartTestHarness();

            await harness.Bus.Publish(new RedeliverySubjects.StartCommand()
            {
                ActivityAExecutionDelay = 1000,
                ActivityBExecutionDelay = 1000,
                ActivityCExecutionDelay = 1000,
                ActivityAThrowOnExecution = false,
                ActivityBThrowOnExecution = false,
                ActivityCThrowOnExecution = true,
                ActivityAThrowOnCompensation = false,
                ActivityBThrowOnCompensation = true,
                ShouldThrowIgnoredException = false
            });

            Assert.That(await harness.Published.Any<RoutingSlipCompensationFailed>());
        }
    }


    namespace RedeliverySubjects
    {
        using System;
        using Microsoft.Extensions.Logging;


        public record StartCommand
        {
            public int ActivityAExecutionDelay { get; set; }
            public int ActivityBExecutionDelay { get; set; }
            public int ActivityCExecutionDelay { get; set; }
            public bool ActivityAThrowOnExecution { get; set; }
            public bool ActivityBThrowOnExecution { get; set; }
            public bool ActivityCThrowOnExecution { get; set; }
            public bool ActivityAThrowOnCompensation { get; set; }
            public bool ActivityBThrowOnCompensation { get; set; }
            public bool ShouldThrowIgnoredException { get; set; }
        }


        public record CommandA
        {
            public bool ShouldThrowException { get; set; }
            public bool ShouldThrowIgnoredException { get; set; }
            public int MillisecondsDelay { get; set; }
            public bool ShouldThrowInCompensation { get; set; }
        }


        public record CompensateA
        {
            public bool ShouldThrow { get; set; }
            public bool ShouldThrowIgnoredException { get; set; }
        }


        public record CommandB
        {
            public bool ShouldThrowException { get; set; }
            public bool ShouldThrowIgnoredException { get; set; }
            public int MillisecondsDelay { get; set; }
            public bool ShouldThrowInCompensation { get; set; }
        }


        public record CompensateB
        {
            public bool ShouldThrow { get; set; }
            public bool ShouldThrowIgnoredException { get; set; }
        }


        public record CommandC
        {
            public bool ShouldThrowException { get; set; }
            public bool ShouldThrowIgnoredException { get; set; }
            public int MillisecondsDelay { get; set; }
            public bool ShouldThrowInCompensation { get; set; }
        }


        public class BusinessException :
            Exception
        {
            public BusinessException()
            {
            }

            public BusinessException(string message)
                : base(message)
            {
            }
        }


        public class ActivityA :
            IActivity<CommandA, CompensateA>
        {
            readonly ILogger<ActivityA> _logger;

            public ActivityA(ILogger<ActivityA> logger)
            {
                _logger = logger;
            }

            public async Task<ExecutionResult> Execute(ExecuteContext<CommandA> context)
            {
                _logger.LogInformation("Executing transaction {StepName} | Retry: {RetryCount} - Redelivery Count: {RedeliveryCount}", nameof(ActivityA),
                    context.GetRetryCount(), context.GetRedeliveryCount());

                if (context.Arguments.ShouldThrowException)
                {
                    if (context.Arguments.ShouldThrowIgnoredException)
                        throw new BusinessException($"An error has occurred executing {nameof(CommandA)}");

                    throw new Exception($"An error has occurred executing {nameof(CommandA)}");
                }

                await Task.Delay(context.Arguments.MillisecondsDelay);

                return context.Completed(new CompensateA
                {
                    ShouldThrow = context.Arguments.ShouldThrowInCompensation,
                    ShouldThrowIgnoredException = context.Arguments.ShouldThrowIgnoredException
                });
            }

            public Task<CompensationResult> Compensate(CompensateContext<CompensateA> context)
            {
                _logger.LogCritical("Compensating transaction {StepName} | Retry: {RetryCount} - Redelivery Count: {RedeliveryCount}", nameof(ActivityA),
                    context.GetRetryCount(), context.GetRedeliveryCount());

                if (context.Log.ShouldThrow)
                {
                    if (context.Log.ShouldThrowIgnoredException)
                        throw new BusinessException($"An error has occurred executing {nameof(CompensateA)}");

                    throw new Exception($"An error has occurred executing {nameof(CompensateA)}");
                }

                return Task.FromResult(context.Compensated());
            }
        }


        public class ActivityB :
            IActivity<CommandB, CompensateB>
        {
            readonly ILogger<ActivityB> _logger;

            public ActivityB(ILogger<ActivityB> logger)
            {
                _logger = logger;
            }

            public async Task<ExecutionResult> Execute(ExecuteContext<CommandB> context)
            {
                _logger.LogInformation("Executing transaction {StepName} | Retry: {RetryCount} - Redelivery Count: {RedeliveryCount}", nameof(ActivityB),
                    context.GetRetryCount(), context.GetRedeliveryCount());

                if (context.Arguments.ShouldThrowException)
                {
                    if (context.Arguments.ShouldThrowIgnoredException)
                        throw new BusinessException($"An error has occurred executing {nameof(CommandB)}");

                    throw new Exception($"An error has occurred executing {nameof(CommandB)}");
                }

                await Task.Delay(context.Arguments.MillisecondsDelay);

                return context.Completed(new CompensateB
                {
                    ShouldThrow = context.Arguments.ShouldThrowInCompensation,
                    ShouldThrowIgnoredException = context.Arguments.ShouldThrowIgnoredException
                });
            }

            public Task<CompensationResult> Compensate(CompensateContext<CompensateB> context)
            {
                _logger.LogCritical("Compensating transaction {StepName} | Retry: {RetryCount} - Redelivery Count: {RedeliveryCount}", nameof(ActivityB),
                    context.GetRetryCount(), context.GetRedeliveryCount());

                if (context.Log.ShouldThrow)
                {
                    if (context.Log.ShouldThrowIgnoredException)
                        throw new BusinessException($"An error has occurred executing {nameof(CompensateB)}");

                    throw new Exception($"An error has occurred executing {nameof(CompensateB)}");
                }

                return Task.FromResult(context.Compensated());
            }
        }


        public class ActivityC :
            IExecuteActivity<CommandC>
        {
            readonly ILogger<ActivityC> _logger;

            public ActivityC(ILogger<ActivityC> logger)
            {
                _logger = logger;
            }

            public async Task<ExecutionResult> Execute(ExecuteContext<CommandC> context)
            {
                _logger.LogInformation("Executing transaction {StepName} | Retry: {RetryCount} - Redelivery Count: {RedeliveryCount}", nameof(ActivityC),
                    context.GetRetryCount(), context.GetRedeliveryCount());

                if (context.Arguments.ShouldThrowException)
                {
                    if (context.Arguments.ShouldThrowIgnoredException)
                        throw new BusinessException($"An error has occurred executing {nameof(CommandC)}");

                    throw new Exception($"An error has occurred executing {nameof(CommandC)}");
                }

                await Task.Delay(context.Arguments.MillisecondsDelay);

                return context.Completed();
            }
        }


        public class StartCommandConsumer :
            IConsumer<StartCommand>
        {
            readonly ILogger<StartCommandConsumer> _logger;
            readonly IRoutingSlipExecutor _executor;
            readonly IEndpointAddressProvider _addressProvider;

            public StartCommandConsumer(ILogger<StartCommandConsumer> logger, IRoutingSlipExecutor executor, IEndpointAddressProvider addressProvider)
            {
                _logger = logger;
                _executor = executor;
                _addressProvider = addressProvider;
            }

            public async Task Consume(ConsumeContext<StartCommand> context)
            {
                _logger.LogDebug("Initiating RoutingSlip...");

                var builder = new RoutingSlipBuilder(NewId.NextGuid());
                builder.AddActivity(nameof(ActivityA), _addressProvider.GetExecuteEndpoint<ActivityA, CommandA>(),
                    new CommandA
                    {
                        MillisecondsDelay = context.Message.ActivityAExecutionDelay,
                        ShouldThrowException = context.Message.ActivityAThrowOnExecution,
                        ShouldThrowInCompensation = context.Message.ActivityAThrowOnCompensation,
                        ShouldThrowIgnoredException = context.Message.ShouldThrowIgnoredException
                    });
                builder.AddActivity(nameof(ActivityB), _addressProvider.GetExecuteEndpoint<ActivityB, CommandB>(),
                    new CommandB
                    {
                        MillisecondsDelay = context.Message.ActivityBExecutionDelay,
                        ShouldThrowException = context.Message.ActivityBThrowOnExecution,
                        ShouldThrowInCompensation = context.Message.ActivityBThrowOnCompensation,
                        ShouldThrowIgnoredException = context.Message.ShouldThrowIgnoredException
                    });
                builder.AddActivity(nameof(ActivityC), _addressProvider.GetExecuteEndpoint<ActivityC, CommandC>(),
                    new CommandC
                    {
                        MillisecondsDelay = context.Message.ActivityCExecutionDelay,
                        ShouldThrowException = context.Message.ActivityCThrowOnExecution,
                        ShouldThrowIgnoredException = context.Message.ShouldThrowIgnoredException
                    });

                var routingSlip = builder.Build();

                await _executor.Execute(routingSlip);
            }
        }


        public interface IEndpointAddressProvider
        {
            Uri GetExecuteEndpoint<T, TArguments>()
                where T : class, IExecuteActivity<TArguments>
                where TArguments : class;
        }


        public class InMemoryEndpointAddressProvider :
            IEndpointAddressProvider
        {
            readonly IEndpointNameFormatter _formatter;

            public InMemoryEndpointAddressProvider(IEndpointNameFormatter formatter)
            {
                _formatter = formatter;
            }

            public Uri GetExecuteEndpoint<T, TArguments>()
                where T : class, IExecuteActivity<TArguments>
                where TArguments : class
            {
                return new Uri($"exchange:{_formatter.ExecuteActivity<T, TArguments>()}");
            }
        }
    }
}
