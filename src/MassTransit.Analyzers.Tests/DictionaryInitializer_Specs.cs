using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace MassTransit.Analyzers.Tests
{
    public class DictionaryInitializer_Specs :
        CodeFixVerifier
    {
        readonly string Usings = @"
using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit;
";

        readonly string MessageContracts = @"
namespace ConsoleApplication1
{
    public interface OrderSubmitted
    {
        Guid Id { get; }
        string CustomerId { get; }
        IReadOnlyDictionary<string, OrderItem> OrderItems { get; }
    }

    public interface SubmitOrder
    {
        Guid Id { get; }
        string CustomerId { get; }
        IReadOnlyList<OrderItem> OrderItems { get; }
    }

    public interface OrderItem
    {
        Guid Id { get; }
        Product Product { get; }
        int Quantity { get; }
        decimal Price { get; }
    }

    public interface Product
    {
        string Name { get; }
        Uri Category { get; }
    }
}
";

        readonly string Dtos = @"
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; }
        public IDictionary<string, OrderItemDto> OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public ProductDto Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
    }
";

        [Test]
        public void WhenTypesAreStructurallyCompatibleAndMissingProperty_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
            {
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0003",
                Message =
                    "Anonymous type is missing properties that are in the message contract 'OrderSubmitted'. The following properties are missing: Id, CustomerId, OrderItems",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 46, 47)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenTypesAreStructurallyIncompatible_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
            {
                InVar.Id,
                CustomerId = ""53051996-AEEC-4EF1-BCFD-7835F17BA8E7"",
                OrderItems = new []
                {
                    new
                    {
                        Id = ""53051996-AEEC-4EF1-BCFD-7835F17BA8E9"",
                        Product = new
                        {
                            Name = ""Pencil"",
                            Category = ""category:office""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message =
                    "Anonymous type does not map to message contract 'OrderSubmitted'. The following properties of the anonymous type are incompatible: OrderItems",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 46, 47)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenTypesAreStructurallyCompatible_ShouldHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<OrderSubmitted>(new
            {
                InVar.Id,
                CustomerId = ""53051996-AEEC-4EF1-BCFD-7835F17BA8E7"",
                OrderItems = new []
                {
                    new
                    {
                        Id = ""53051996-AEEC-4EF1-BCFD-7835F17BA8E9"",
                        Product = new
                        {
                            Name = ""Pencil"",
                            Category = ""category:office""
                        },
                        Quantity = 10,
                        Price = 10.0m
                    }
                }.ToDictionary(x => x.Id)
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
            return new MessageContractAnalyzer();
        }
    }
}
