#nullable enable
namespace MassTransit.Analyzers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Operations;


    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CancellationTokenOverloadMethodAnalyzer :
        DiagnosticAnalyzer
    {
        public const string CancellationTokenOverloadMethodRuleId = "MCA2016";

        const string Category = "Reliability";
        internal const string ParameterIndex = "ParameterIndex";
        internal const string ParameterName = "ParameterName";
        internal const string CancellationTokens = "CancellationTokens";

        static readonly DiagnosticDescriptor CancellationTokenOverloadMethodRule = new DiagnosticDescriptor(CancellationTokenOverloadMethodRuleId,
            "Context.CancellationToken could be used in method overload with CancellationToken",
            "Cancellation token from '{0}' can be used in cancellation token overload for '{1}' method",
            Category, DiagnosticSeverity.Info, true,
            "Context.CancellationToken can be passed in method with overload.");

        readonly ConcurrentDictionary<ISymbol, IEnumerable<ISymbol>> _membersByType =
            new ConcurrentDictionary<ISymbol, IEnumerable<ISymbol>>(SymbolEqualityComparer.Default);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(CancellationTokenOverloadMethodRule);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterCompilationStartAction(AnalyzeCompilationStart);
        }

        void AnalyzeCompilationStart(CompilationStartAnalysisContext context)
        {
            var cancellationTokenSymbol = GetBestTypeByMetadataName(context.Compilation, "System.Threading.CancellationToken");
            var pipeContextTypeSymbol = GetBestTypeByMetadataName(context.Compilation, "MassTransit.PipeContext");
            var cancellationTokenSourceSymbol = GetBestTypeByMetadataName(context.Compilation, "System.Threading.CancellationTokenSource");
            if (cancellationTokenSymbol == null || pipeContextTypeSymbol == null)
                return;

            var consumeContextTypeSymbol = GetBestTypeByMetadataName(context.Compilation, "MassTransit.ConsumeContext");

            context.RegisterOperationAction(analysisContext =>
            {
                var invocation = (IInvocationOperation)analysisContext.Operation;

                if (!(analysisContext.ContainingSymbol is IMethodSymbol))
                    return;

                if (IsConsumeContextCall(invocation, analysisContext.Compilation, consumeContextTypeSymbol, analysisContext.CancellationToken))
                    return;

                if (!HasAnOverloadWithCancellationToken(invocation, cancellationTokenSymbol, cancellationTokenSourceSymbol, out var newParameterIndex,
                        out var newParameterName))
                    return;

                var availableCancellationTokens = FindCancellationTokens(invocation, cancellationTokenSymbol, pipeContextTypeSymbol, context.CancellationToken);

                if (!availableCancellationTokens.Any())
                    return;

                var methodNameNode = GetInvocationMethodNameNode(analysisContext.Operation.Syntax) ?? analysisContext.Operation.Syntax;

                ImmutableDictionary<string, string?> properties = ImmutableDictionary.Create<string, string?>(StringComparer.Ordinal)
                    .Add(ParameterIndex, newParameterIndex.ToString(CultureInfo.InvariantCulture))
                    .Add(ParameterName, newParameterName)
                    .Add(CancellationTokens, string.Join(",", availableCancellationTokens));

                var diagnostic = Diagnostic.Create(CancellationTokenOverloadMethodRule, invocation.Syntax.GetLocation(), properties,
                    string.Join(",", availableCancellationTokens), methodNameNode);

                analysisContext.ReportDiagnostic(diagnostic);
            }, OperationKind.Invocation);
        }

        static bool IsConsumeContextCall(IInvocationOperation operation, Compilation compilation, ISymbol? consumeContextTypeSymbol,
            CancellationToken cancellationToken)
        {
            if (consumeContextTypeSymbol == null)
                return false;

            var receiverType = operation.GetReceiverType(compilation, true, cancellationToken);
            return receiverType != null && TryGetInterface(receiverType, consumeContextTypeSymbol, out _);
        }

        static bool TryGetInterface(ITypeSymbol? symbol, ISymbol expectedSymbol, out ITypeSymbol? result)
        {
            result = null;
            if (symbol == null || !SymbolEqualityComparer.Default.Equals(symbol.ContainingNamespace, expectedSymbol.ContainingNamespace))
                return false;

            if (SymbolEqualityComparer.Default.Equals(symbol, expectedSymbol))
            {
                result = symbol;
                return true;
            }

            foreach (var s in symbol.Interfaces)
            {
                if (TryGetInterface(s, expectedSymbol, out result))
                    return true;
            }

            return false;
        }

        static bool HasAnOverloadWithCancellationToken(IInvocationOperation operation, ITypeSymbol cancellationTokenSymbol,
            ISymbol? cancellationTokenSourceSymbol, out int parameterIndex, out string? parameterName)
        {
            parameterName = null;
            parameterIndex = -1;
            var method = operation.TargetMethod;
            if (method.Name == nameof(CancellationTokenSource.CreateLinkedTokenSource)
                && SymbolEqualityComparer.Default.Equals(method.ContainingType, cancellationTokenSourceSymbol))
                return false;

            if (IsArgumentImplicitlyDeclared(operation, cancellationTokenSymbol, out parameterIndex, out parameterName))
                return true;

            var overload = FindOverloadWithAdditionalParameterOfType(operation.TargetMethod, cancellationTokenSymbol);
            if (overload == null)
                return false;

            for (var i = 0; i < overload.Parameters.Length; i++)
            {
                if (!SymbolEqualityComparer.Default.Equals(overload.Parameters[i].Type, cancellationTokenSymbol))
                    continue;
                parameterName ??= overload.Parameters[i].Name;
                parameterIndex = i;
                break;
            }

            return true;


            static bool IsArgumentImplicitlyDeclared(IInvocationOperation invocationOperation, ISymbol cancellationTokenSymbol, out int parameterIndex,
                out string? parameterName)
            {
                parameterIndex = -1;
                parameterName = null;

                static bool IsValid(IArgumentOperation arg, ISymbol cancellationTokenSymbol)
                {
                    return arg is { IsImplicit: true, Parameter: { } } && SymbolEqualityComparer.Default.Equals(arg.Parameter.Type, cancellationTokenSymbol);
                }

                foreach (var arg in invocationOperation.Arguments.Where(x => IsValid(x, cancellationTokenSymbol)))
                {
                    if (arg.Parameter == null)
                        continue;

                    parameterIndex = invocationOperation.TargetMethod.Parameters.IndexOf(arg.Parameter);
                    parameterName = arg.Parameter.Name;

                    return true;
                }

                return false;
            }
        }

        static IMethodSymbol? FindOverloadWithAdditionalParameterOfType(IMethodSymbol methodSymbol, params ITypeSymbol?[] additionalParameterTypes)
        {
            additionalParameterTypes = additionalParameterTypes.Where(type => type != null).ToArray();
            if (additionalParameterTypes.Length == 0)
                return null;

            ImmutableArray<ISymbol> members;

            members = methodSymbol.ContainingType.GetMembers(methodSymbol.Name);

            return members.OfType<IMethodSymbol>()
                .FirstOrDefault(member => HasSimilarParameters(methodSymbol, member, additionalParameterTypes));
        }

        static bool HasSimilarParameters(IMethodSymbol methodSymbol, IMethodSymbol otherMethod, params ITypeSymbol?[] additionalParameterTypes)
        {
            if (SymbolEqualityComparer.Default.Equals(methodSymbol, otherMethod))
                return false;

            List<ITypeSymbol>? methodParameters = methodSymbol.Parameters.Select(p => p.Type).ToList();
            List<ITypeSymbol>? otherMethodParameters = otherMethod.Parameters.Select(p => p.Type).ToList();

            if (otherMethodParameters.Count - methodParameters.Count != additionalParameterTypes.Length)
                return false;

            foreach (var param in methodParameters)
                otherMethodParameters.Remove(param);

            foreach (var param in additionalParameterTypes)
                otherMethodParameters.Remove(param!);

            return !otherMethodParameters.Any();
        }

        // Check if there's a method overload with the same parameters as this one, in the same order, plus a ct at the end.

        static SyntaxNode? GetInvocationMethodNameNode(SyntaxNode invocationNode)
        {
            if (!(invocationNode is InvocationExpressionSyntax invocationExpression))
                return null;

            if (invocationExpression.Expression is MemberBindingExpressionSyntax memberBindingExpression)
            {
                // When using nullability features, specifically attempting to dereference possible null references,
                // the dot becomes part of the member invocation expression, so we need to return just the name,
                // so that the diagnostic gets properly returned in the method name only.
                return memberBindingExpression.Name;
            }

            return invocationExpression.Expression;
        }

        static ImmutableArray<INamedTypeSymbol> GetTypesByMetadataName(Compilation compilation, string typeMetadataName)
        {
            var result = ImmutableArray.CreateBuilder<INamedTypeSymbol>();
            var symbol = compilation.Assembly.GetTypeByMetadataName(typeMetadataName);
            if (symbol != null)
                result.Add(symbol);

            foreach (var reference in compilation.References)
            {
                if (!(compilation.GetAssemblyOrModuleSymbol(reference) is IAssemblySymbol assemblySymbol))
                    continue;

                symbol = assemblySymbol.GetTypeByMetadataName(typeMetadataName);
                if (symbol != null)
                    result.Add(symbol);
            }

            return result.ToImmutable();
        }

        static INamedTypeSymbol? GetBestTypeByMetadataName(Compilation compilation, string fullyQualifiedMetadataName)
        {
            INamedTypeSymbol? type = null;

            foreach (var currentType in GetTypesByMetadataName(compilation, fullyQualifiedMetadataName))
            {
                if (ReferenceEquals(currentType.ContainingAssembly, compilation.Assembly))
                    return currentType;

                switch (GetResultantVisibility(currentType))
                {
                    case SymbolVisibility.Public:
                    case SymbolVisibility.Internal when currentType.ContainingAssembly.GivesAccessTo(compilation.Assembly):
                        break;

                    default:
                        continue;
                }

                if (type is not null)
                {
                    // Multiple visible types with the same metadata name are present
                    return null;
                }

                type = currentType;
            }

            return type;
        }

        // https://github.com/dotnet/roslyn/blob/d2ff1d83e8fde6165531ad83f0e5b1ae95908289/src/Workspaces/SharedUtilitiesAndExtensions/Compiler/Core/Extensions/ISymbolExtensions.cs#L28-L73
        static SymbolVisibility GetResultantVisibility(ISymbol symbol)
        {
            // Start by assuming it's visible.
            var visibility = SymbolVisibility.Public;
            switch (symbol.Kind)
            {
                case SymbolKind.Alias:
                    // Aliases are uber private.  They're only visible in the same file that they
                    // were declared in.
                    return SymbolVisibility.Private;
                case SymbolKind.Parameter:
                    // Parameters are only as visible as their containing symbol
                    return GetResultantVisibility(symbol.ContainingSymbol);
                case SymbolKind.TypeParameter:
                    // Type Parameters are private.
                    return SymbolVisibility.Private;
            }

            while (symbol != null && symbol.Kind != SymbolKind.Namespace)
            {
                switch (symbol.DeclaredAccessibility)
                {
                    // If we see anything private, then the symbol is private.
                    case Accessibility.NotApplicable:
                    case Accessibility.Private:
                        return SymbolVisibility.Private;
                    // If we see anything internal, then knock it down from public to
                    // internal.
                    case Accessibility.Internal:
                    case Accessibility.ProtectedAndInternal:
                        visibility = SymbolVisibility.Internal;
                        break;
                    // For anything else (Public, Protected, ProtectedOrInternal), the
                    // symbol stays at the level we've gotten so far.
                }

                symbol = symbol.ContainingSymbol;
            }

            return visibility;
        }

        string[] FindCancellationTokens(IOperation operation, INamedTypeSymbol cancellationTokenSymbol, INamedTypeSymbol pipeContextSymbol,
            CancellationToken cancellationToken)
        {
            var isStatic = operation.IsStaticMember(cancellationToken);
            var paths = new HashSet<string>(StringComparer.Ordinal);

            foreach (var availableSymbol in operation.GetParameters(cancellationToken))
            {
                foreach (var member in GetMembers(availableSymbol.TypeSymbol, cancellationTokenSymbol, pipeContextSymbol))
                {
                    if (!IsSymbolAccessibleFromOperation(member, operation))
                        continue;

                    if (availableSymbol.Name == null && isStatic && !member.IsStatic)
                        continue;

                    var fullPath = ComputeFullPath(availableSymbol.Name, member);
                    paths.Add(fullPath);
                }
            }

            return paths.Count == 0
                ? []
                : paths.OrderBy(value => value.Count(c => c == '.')).ThenBy(value => value, StringComparer.Ordinal).ToArray();

            static string ComputeFullPath(string? prefix, ISymbol symbols)
            {
                if (prefix == null)
                    return symbols.Name;

                if (string.IsNullOrEmpty(symbols.Name))
                    return prefix;

                return prefix + "." + symbols.Name;
            }

            static bool IsSymbolAccessibleFromOperation(ISymbol symbol, IOperation operation)
            {
                return operation.SemanticModel!.IsAccessible(operation.Syntax.Span.Start, symbol);
            }
        }

        IEnumerable<ISymbol> GetMembers(ITypeSymbol symbol, ISymbol cancellationTokenSymbol, ISymbol pipeContextSymbol)
        {
            return _membersByType.GetOrAdd(symbol, _ =>
            {
                // quickly skips some basic types that are known to not contain CancellationToken
                if ((int)symbol.SpecialType >= 1 && (int)symbol.SpecialType <= 45)
                    return [];

                var result = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
                foreach (var member in GetPipeContextMembers(symbol, pipeContextSymbol))
                {
                    switch (member)
                    {
                        case IPropertySymbol propertySymbol when SymbolEqualityComparer.Default.Equals(propertySymbol.Type, cancellationTokenSymbol):
                            result.Add(propertySymbol.Type);
                            break;

                        case IFieldSymbol fieldSymbol when SymbolEqualityComparer.Default.Equals(fieldSymbol.Type, cancellationTokenSymbol):
                            result.Add(fieldSymbol.Type);
                            break;
                    }
                }

                return result;
            });
        }

        static HashSet<ISymbol> GetPipeContextMembers(ITypeSymbol? symbol, ISymbol pipeContextSymbol)
        {
            if (symbol == null)
                return new HashSet<ISymbol>(SymbolEqualityComparer.Default);

            static bool Filter(ISymbol symbol)
            {
                return symbol is { IsImplicitlyDeclared: false, Kind: SymbolKind.Property };
            }

            if (TryGetInterface(symbol, pipeContextSymbol, out var result) && result != null)
                return new HashSet<ISymbol>(result.GetMembers().Where(Filter), SymbolEqualityComparer.Default);

            return new HashSet<ISymbol>(SymbolEqualityComparer.Default);
        }


        enum SymbolVisibility
        {
            Public,
            Internal,
            Private,
        }
    }
}
