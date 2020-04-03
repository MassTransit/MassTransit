namespace MassTransit.Analyzers.Tests
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;


    public class MessageDataInitializer_Specs :
        CodeFixVerifier
    {
        readonly string MessageContracts = @"
namespace ConsoleApplication1
{
    public interface ProcessDocument
    {
        Guid Id { get; }
        string CustomerId { get; }
        MessageData<string> Document { get; }
    }
}
";

        readonly string Usings = @"
using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit;
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

            await bus.Publish<ProcessDocument>(new
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
                    "Anonymous type is missing properties that are in the message contract 'ProcessDocument'. The following properties are missing: Id, CustomerId, Document",
                Severity = DiagnosticSeverity.Info,
                Locations =
                    new[] {new DiagnosticResultLocation("Test0.cs", 25, 48)}
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

            await bus.Publish<ProcessDocument>(new
            {
                InVar.Id,
                CustomerId = ""53051996-AEEC-4EF1-BCFD-7835F17BA8E7"",
                Document = 72
            });
        }
    }
}
";
            var expected = new DiagnosticResult
            {
                Id = "MCA0001",
                Message =
                    "Anonymous type does not map to message contract 'ProcessDocument'. The following properties of the anonymous type are incompatible: Document",
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {new DiagnosticResultLocation("Test0.cs", 25, 48)}
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Test]
        public void WhenTypesAreStructurallyCompatible_ShouldNotHaveDiagnostic()
        {
            var test = Usings + MessageContracts + @"
namespace ConsoleApplication1
{
    class Program
    {
        static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingInMemory(cfg => { });

            await bus.Publish<ProcessDocument>(new
            {
                InVar.Id,
                CustomerId = ""53051996-AEEC-4EF1-BCFD-7835F17BA8E7"",
                Document = ""This would be a really big document typically""
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
