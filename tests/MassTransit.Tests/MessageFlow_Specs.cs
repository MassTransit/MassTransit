namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;


    [TestFixture]
    [Explicit]
    public class MessageFlow_Specs
    {
        [Test]
        public async Task Should_generate_a_graph_of_the_message_flow()
        {
            var harness = new InMemoryTestHarness
            {
                TestTimeout = TimeSpan.FromSeconds(2),
                TestInactivityTimeout = TimeSpan.FromSeconds(2)
            };

            harness.Consumer(() => new AFooConsumer());
            harness.Consumer(() => new BFooConsumer());
            harness.Consumer(() => new CFooConsumer());
            harness.Consumer(() => new DFooConsumer());
            harness.Consumer(() => new EFooConsumer());

            EndpointConvention.Map<EFoo>(harness.InputQueueAddress);

            await harness.Start();

            await harness.Bus.Publish<AFoo>(new {InVar.CorrelationId});

            await harness.Bus.Publish<BFoo>(new {InVar.CorrelationId});

            await harness.OutputTimeline(TestContext.Out, options => options.IncludeAddress());

            await harness.Stop();
        }
    }


    public interface ICorrelated
    {
        public Guid CorrelationId { get; set; }
    }


    public interface AFoo : ICorrelated
    {
    }


    public interface BFoo : ICorrelated
    {
    }


    public interface CFoo : ICorrelated
    {
    }


    public interface DFoo : ICorrelated
    {
    }


    public interface EFoo : ICorrelated
    {
    }


    public class AFooConsumer : IConsumer<AFoo>
    {
        public async Task Consume(ConsumeContext<AFoo> context)
        {
            await context.Publish<BFoo>(new {CorrelationId = Guid.NewGuid()});

            await context.Publish<BFoo>(new {CorrelationId = Guid.NewGuid()});
        }
    }


    public class BFooConsumer : IConsumer<BFoo>
    {
        public async Task Consume(ConsumeContext<BFoo> context)
        {
            await context.Publish<CFoo>(new {CorrelationId = Guid.NewGuid()});

            await context.Send<EFoo>(new {CorrelationId = Guid.NewGuid()});
        }
    }


    public class CFooConsumer : IConsumer<CFoo>
    {
        public async Task Consume(ConsumeContext<CFoo> context)
        {
            await context.Publish<DFoo>(new {CorrelationId = Guid.NewGuid()});

            await context.Publish<DFoo>(new {CorrelationId = Guid.NewGuid()});
        }
    }


    public class DFooConsumer : IConsumer<DFoo>
    {
        public Task Consume(ConsumeContext<DFoo> context)
        {
            return Task.CompletedTask;
        }
    }


    public class EFooConsumer : IConsumer<EFoo>
    {
        public async Task Consume(ConsumeContext<EFoo> context)
        {
            await context.Publish<DFoo>(new {CorrelationId = Guid.NewGuid()});
        }
    }
}
