namespace MassTransit.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;


    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncMethodAnalyzer :
        DiagnosticAnalyzer
    {
        public const string MissingAwaitRuleId = "MTA0001";

        const string Category = "Usage";

        static readonly DiagnosticDescriptor MissingAwaitRule = new DiagnosticDescriptor(MissingAwaitRuleId,
            "MassTransit method is not awaited or captured",
            "Method {0} is not awaited or captured and may result in message loss",
            Category, DiagnosticSeverity.Warning, true,
            "MassTransit method is not awaited or captured");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(MissingAwaitRule);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;

            var symbol = context.SemanticModel.GetSymbolInfo(invocationExpression);
            if (symbol.Symbol?.Kind == SymbolKind.Method)
            {
                var methodSymbol = (IMethodSymbol)symbol.Symbol;

                if (_awaitableMethods.Contains($"{methodSymbol.ContainingNamespace}.{methodSymbol.ContainingType.Name}.{methodSymbol.Name}"))
                {
                    if (invocationExpression.Parent is ExpressionStatementSyntax)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(MissingAwaitRule, invocationExpression.GetLocation(),
                            SymbolDisplay.ToDisplayString(methodSymbol,
                                SymbolDisplayFormat.CSharpShortErrorMessageFormat.WithParameterOptions(SymbolDisplayParameterOptions.None))));
                    }
                }
            }
        }

        static readonly HashSet<string> _awaitableMethods = InitializeAwaitableMethods();

        static HashSet<string> InitializeAwaitableMethods()
        {
            return new HashSet<string>
            {
                "MassTransit.ISendEndpoint.Send",
                "MassTransit.IPublishEndpoint.Publish",
                "MassTransit.ConsumeContext.RespondAsync",
                "MassTransit.ConsumeContextExtensions.Forward",
                "MassTransit.ConsumeContextSelfSchedulerExtensions.ScheduleSend",
                "MassTransit.EndpointConventionExtensions.Send",
                "MassTransit.IRequestClient.Request",
                "MassTransit.IRequestClient.GetResponse",
                "MassTransit.PublishContextExecuteExtensions.Publish",
                "MassTransit.RequestHandle.GetResponse",
                "MassTransit.PublishEndpointRecurringSchedulerExtensions.ScheduleRecurringSend",
                "MassTransit.PublishEndpointSchedulerExtensions.ScheduleSend",
                "MassTransit.RedeliverExtensions.Redeliver",
                "MassTransit.RequestClientExtensions.Request",
                "MassTransit.RequestExtensions.Request",
                "MassTransit.RespondAsyncExecuteExtensions.RespondAsync",
                "MassTransit.SendContextExecuteExtensions.Send",
                "MassTransit.SendEndpointExtensions.Send",
                "MassTransit.SendEndpointRecurringSchedulerExtensions.ScheduleRecurringSend",
                "MassTransit.SendEndpointSchedulerExtensions.ScheduleSend",
                "MassTransit.TimeSpanContextScheduleExtensions.ScheduleSend",
                "MassTransit.TimeSpanScheduleExtensions.ScheduleSend"
            };
        }
    }
}
