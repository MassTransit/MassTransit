#nullable enable
namespace MassTransit.Analyzers.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Operations;


    static class OperationExtensions
    {
        public static ITypeSymbol? GetReceiverType(this IInvocationOperation invocation, Compilation compilation, bool beforeConversion,
            CancellationToken cancellationToken)
        {
            if (invocation.Instance != null)
                return beforeConversion ? GetReceiverType(invocation.Instance.Syntax, compilation, cancellationToken) : invocation.Instance.Type;

            if (!invocation.TargetMethod.IsExtensionMethod || invocation.TargetMethod.Parameters.IsEmpty)
                return null;
            var firstArg = invocation.Arguments.FirstOrDefault();
            if (firstArg != null)
                return beforeConversion ? GetReceiverType(firstArg.Value.Syntax, compilation, cancellationToken) : firstArg.Value.Type;

            return invocation.TargetMethod.Parameters[0].IsParams ? invocation.TargetMethod.Parameters[0].Type : null;
        }

        static ITypeSymbol? GetReceiverType(SyntaxNode receiverSyntax, Compilation compilation, CancellationToken cancellationToken)
        {
            var model = compilation.GetSemanticModel(receiverSyntax.SyntaxTree);
            var typeInfo = model.GetTypeInfo(receiverSyntax, cancellationToken);
            return typeInfo.Type;
        }

        public static List<NameAndType> GetParameters(this IOperation operation, CancellationToken cancellationToken)
        {
            var result = new List<NameAndType>();
            var semanticModel = operation.SemanticModel!;
            var node = operation.Syntax;

            while (node != null)
            {
                switch (node)
                {
                    case AccessorDeclarationSyntax accessor:
                    {
                        if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
                        {
                            var property = node.Ancestors().OfType<PropertyDeclarationSyntax>().FirstOrDefault();
                            if (property != null)
                            {
                                var symbol = operation.SemanticModel.GetDeclaredSymbol(property, cancellationToken);
                                if (symbol != null)
                                    result.Add(new NameAndType("value", symbol.Type));
                            }
                        }

                        break;
                    }

                    case PropertyDeclarationSyntax _:
                        return result;

                    case IndexerDeclarationSyntax indexerDeclarationSyntax:
                    {
                        var symbol = semanticModel.GetDeclaredSymbol(indexerDeclarationSyntax, cancellationToken);
                        if (symbol != null)
                            result.AddRange(symbol.Parameters.Select(parameter => new NameAndType(parameter.Name, parameter.Type)));

                        return result;
                    }

                    case MethodDeclarationSyntax methodDeclaration:
                    {
                        var symbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);
                        if (symbol != null)
                            result.AddRange(symbol.Parameters.Select(parameter => new NameAndType(parameter.Name, parameter.Type)));

                        return result;
                    }
                    case LocalFunctionStatementSyntax localFunctionStatement:
                    {
                        if (semanticModel.GetDeclaredSymbol(localFunctionStatement, cancellationToken) is IMethodSymbol symbol)
                            result.AddRange(symbol.Parameters.Select(parameter => new NameAndType(parameter.Name, parameter.Type)));

                        break;
                    }

                    case ConstructorDeclarationSyntax constructorDeclaration:
                    {
                        var symbol = semanticModel.GetDeclaredSymbol(constructorDeclaration, cancellationToken);
                        if (symbol != null)
                            result.AddRange(symbol.Parameters.Select(parameter => new NameAndType(parameter.Name, parameter.Type)));

                        return result;
                    }
                }

                node = node.Parent;
            }

            return result;
        }

        public static bool IsStaticMember(this IOperation operation, CancellationToken cancellationToken)
        {
            var memberDeclarationSyntax = operation.Syntax.Ancestors().FirstOrDefault(syntax => syntax is MemberDeclarationSyntax);
            if (memberDeclarationSyntax == null)
                return false;

            var symbol = operation.SemanticModel!.GetDeclaredSymbol(memberDeclarationSyntax, cancellationToken);
            return symbol is { IsStatic: true };
        }


        [StructLayout(LayoutKind.Auto)]
        internal readonly struct NameAndType
        {
            public NameAndType(string name, ITypeSymbol typeSymbol)
            {
                Name = name;
                TypeSymbol = typeSymbol;
            }

            public string Name { get; }
            public ITypeSymbol TypeSymbol { get; }
        }
    }
}
