namespace MassTransit.Analyzers.Tests
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;


    public class Await_Specs :
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
        public void Calling_Publish()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            bus.Publish<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""427"",
            });

            await bus.Publish<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""427"",
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MTA0001",
                Message = "Method IPublishEndpoint.Publish<OrderSubmitted>() is not awaited or captured and may result in message loss",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {new DiagnosticResultLocation("Test0.cs", 31, 13)}
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void Calling_Send()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var endpoint = await bus.GetSendEndpoint(new Uri(""loopback://localhost/input_queue""));
            endpoint.Send<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""427"",
            });

            await endpoint.Send<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""427"",
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MTA0001",
                Message = "Method ISendEndpoint.Send<OrderSubmitted>() is not awaited or captured and may result in message loss",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {new DiagnosticResultLocation("Test0.cs", 32, 13)}
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void Calling_GetResponse()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var client = bus.CreateRequestClient<SubmitOrder>();
            client.GetResponse<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""427"",
            });

            var response = await client.GetResponse<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""427"",
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MTA0001",
                Message = "Method IRequestClient<SubmitOrder>.GetResponse<OrderSubmitted>() is not awaited or captured and may result in message loss",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {new DiagnosticResultLocation("Test0.cs", 32, 13)}
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void Calling_RespondAsync()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Consumer :
        IConsumer<SubmitOrder>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            context.RespondAsync<OrderSubmitted>(context.Message);
        }
    }

    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var client = bus.CreateRequestClient<SubmitOrder>();
            var response = await client.GetResponse<OrderSubmitted>(new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""427"",
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MTA0001",
                Message = "Method ConsumeContext.RespondAsync<OrderSubmitted>() is not awaited or captured and may result in message loss",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {new DiagnosticResultLocation("Test0.cs", 30, 13)}
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void Calling_Create()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            var client = bus.CreateRequestClient<SubmitOrder>();
            client.Create(new
            {
                Id = NewId.NextGuid(),
                CustomerId = ""427"",
            });
        }
    }
}
";
            VerifyCSharpDiagnostic(test);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new MessageContractCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AsyncMethodAnalyzer();
        }
    }
}
