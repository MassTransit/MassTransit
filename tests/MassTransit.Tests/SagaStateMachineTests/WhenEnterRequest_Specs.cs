namespace MassTransit.Tests.SagaStateMachineTests
{
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using WhenEnterSpecs;


    namespace WhenEnterSpecs
    {
        using System;
        using System.Threading.Tasks;
        using Microsoft.Extensions.Logging;


        public class RuleStateMachine : MassTransitStateMachine<Rule>
        {
            public RuleStateMachine(ILogger<RuleStateMachine> logger)
            {
                InstanceState(x => x.CurrentState);

                Request(() => Execute, x => x.ExecutionRequestId, r =>
                {
                    r.ServiceAddress = new Uri("loopback://localhost/ExecuteRule");
                    r.Timeout = TimeSpan.Zero;
                });

                Event(() => CategoryUpdated, evt => evt
                    .CorrelateBy((instance, context) => context.Message.CategoryId == instance.CategoryId)
                    .SelectId(context => NewId.NextGuid()));

                During(Initial,
                    When(CategoryUpdated)
                        .Then(context => logger.LogInformation("Category updated for {Rule}; starting execution...", context.Saga.Format()))
                        .TransitionTo(StartingExecution)
                );

                WhenEnter(StartingExecution, x => x
                    .ThenAsync(async context =>
                    {
                        logger.LogInformation("Starting execution for {Rule}...", context.Saga.Format());
                        context.Saga.ExecutionRequestedAt = DateTime.UtcNow;
                        await Task.Delay(new Random().Next(50, 1000));
                    })
                    .Request(Execute, context => context.Init<ExecuteRule>(new { RuleId = context.Saga.CorrelationId }))
                    .TransitionTo(Executing)
                );

                During(Executing,
                    When(Execute.Completed)
                        .Then(context =>
                        {
                            logger.LogInformation("Executed {Rule}.", context.Saga.Format());
                            context.Saga.ExecutionRequestedAt = null;
                        })
                        .TransitionTo(Waiting),
                    When(Execute.Faulted)
                        .Then(context =>
                        {
                            logger.LogError("Failed to execute {Rule}; retrying later.", context.Saga.Format());
                            context.Saga.ExecutionRequestedAt = null;
                        })
                        .TransitionTo(Waiting)
                );
            }

            public State StartingExecution { get; set; }
            public State Executing { get; set; }
            public State Waiting { get; set; }

            public Event<CategoryUpdated> CategoryUpdated { get; set; }
            public Request<Rule, ExecuteRule, ExecuteRuleResponse> Execute { get; set; }
        }


        public interface CategoryUpdated
        {
            public int CategoryId { get; set; }
        }


        public interface ExecuteRuleResponse
        {
            public Guid RuleId { get; set; }
        }


        public interface ExecuteRule
        {
            public Guid RuleId { get; set; }
        }


        public class Rule :
            SagaStateMachineInstance
        {
            public int CategoryId { get; set; }

            public string CurrentState { get; set; }

            public Guid? ExecutionRequestId { get; set; }
            public DateTime? ExecutionRequestedAt { get; set; }
            public Guid CorrelationId { get; set; }

            public string Format()
            {
                return $"Rule ({CorrelationId})";
            }
        }


        public class ExecuteRuleConsumer :
            IConsumer<ExecuteRule>
        {
            readonly ILogger<ExecuteRuleConsumer> _logger;

            public ExecuteRuleConsumer(ILogger<ExecuteRuleConsumer> logger)
            {
                _logger = logger;
            }

            public async Task Consume(ConsumeContext<ExecuteRule> context)
            {
                _logger.LogInformation("Executing {Rule}", context.Message.RuleId);

                await context.RespondAsync<ExecuteRuleResponse>(new { context.Message.RuleId });
            }
        }
    }


    public class WhenEnterRequest_Specs
    {
        [Test]
        public async Task Should_be_configurable()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<ExecuteRuleConsumer>();
                    x.AddSagaStateMachine<RuleStateMachine, Rule>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var sagaHarness = harness.GetSagaStateMachineHarness<RuleStateMachine,Rule>();

            await harness.Bus.Publish<CategoryUpdated>(new { CategoryId = 27 });

            Assert.That(await sagaHarness.Consumed.Any<ExecuteRuleResponse>(), Is.True);
        }
    }
}
