namespace MassTransit.Analyzers.Tests
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;


    public class CancellationToken_Specs :
        CodeFixVerifier
    {
        const string Usings = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
";

        const string MessageContracts = @"
namespace ConsoleApplication1
{
    public interface OrderSubmitted
    {
        Guid Id { get; }
        string CustomerId { get; }
    }

    public interface SubmitOrder
    {
        Guid Id { get; }
        string CustomerId { get; }
    }
}
";

        [Test]
        public void Calling_task_delay()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Consumer :
        IConsumer<SubmitOrder>
    {
        public Task Consume(ConsumeContext<SubmitOrder> context)
        {
            return Task.Delay(10);
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA2016",
                Message = "Cancellation token from 'context.CancellationToken' can be used in cancellation token overload for 'Task.Delay' method",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 30, 20) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fix = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Consumer :
        IConsumer<SubmitOrder>
    {
        public Task Consume(ConsumeContext<SubmitOrder> context)
        {
            return Task.Delay(10, context.CancellationToken);
        }
    }
}
";

            VerifyCSharpFix(test, fix);
        }

        [Test]
        public void Calling_task_delay_in_activity()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{

        class TestInstance :
            SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
            public string Value { get; set; }
        }

        class SetValueAsyncActivity :
            IStateMachineActivity<TestInstance, SubmitOrder>
        {

        Task IStateMachineActivity<TestInstance, SubmitOrder>.Execute(BehaviorContext<TestInstance, SubmitOrder> context,
            IBehavior<TestInstance, SubmitOrder> next)
        {
            return Task.Delay(10);
        }

        async Task IStateMachineActivity<TestInstance, SubmitOrder>.Faulted<TException>(BehaviorExceptionContext<TestInstance, SubmitOrder, TException> ctx,
            IBehavior<TestInstance, SubmitOrder> next)
        {
            return Task.Run(() => next.Faulted(ctx));
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
";
            var expected1 = new DiagnosticResult
            {
                Id = "MCA2016",
                Message = "Cancellation token from 'context.CancellationToken' can be used in cancellation token overload for 'Task.Delay' method",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 41, 20) }
            };

            var expected2 = new DiagnosticResult
            {
                Id = "MCA2016",
                Message = "Cancellation token from 'ctx.CancellationToken' can be used in cancellation token overload for 'Task.Run' method",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 47, 20) }
            };

            VerifyCSharpDiagnostic(test, expected1, expected2);

            var fix = Usings + MessageContracts + @"
namespace ConsoleApplication1
{

        class TestInstance :
            SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
            public string Value { get; set; }
        }

        class SetValueAsyncActivity :
            IStateMachineActivity<TestInstance, SubmitOrder>
        {

        Task IStateMachineActivity<TestInstance, SubmitOrder>.Execute(BehaviorContext<TestInstance, SubmitOrder> context,
            IBehavior<TestInstance, SubmitOrder> next)
        {
            return Task.Delay(10, context.CancellationToken);
        }

        async Task IStateMachineActivity<TestInstance, SubmitOrder>.Faulted<TException>(BehaviorExceptionContext<TestInstance, SubmitOrder, TException> ctx,
            IBehavior<TestInstance, SubmitOrder> next)
        {
            return Task.Run(() => next.Faulted(ctx), ctx.CancellationToken);
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
";

            VerifyCSharpFix(test, fix);
        }

        [Test]
        public void Calling_task_delay_after_token()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Consumer :
        IConsumer<SubmitOrder>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            await Task.Delay(10, context.CancellationToken);
            context.RespondAsync<OrderSubmitted>(context.Message);
            await Task.Run(() => {});

        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA2016",
                Message = "Cancellation token from 'context.CancellationToken' can be used in cancellation token overload for 'Task.Run' method",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 32, 19) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fix = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Consumer :
        IConsumer<SubmitOrder>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            await Task.Delay(10, context.CancellationToken);
            context.RespondAsync<OrderSubmitted>(context.Message);
            await Task.Run(() => {}, context.CancellationToken);

        }
    }
}
";
            VerifyCSharpFix(test, fix);
        }

        [Test]
        public void Calling_task_delay_with_token()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Consumer :
        IConsumer<SubmitOrder>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            await Task.Delay(10, context.CancellationToken);
            context.RespondAsync<OrderSubmitted>(context.Message);
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void Calling_context_publish_does_not_suggest()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Consumer :
        IConsumer<SubmitOrder>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            return context.Publish<OrderSubmitted>(new {});
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new CancellationTokenOverloadMethodFixer();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new CancellationTokenOverloadMethodAnalyzer();
        }
    }
}
