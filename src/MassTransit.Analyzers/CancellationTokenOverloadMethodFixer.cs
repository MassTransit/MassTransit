namespace MassTransit.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using static CancellationTokenOverloadMethodAnalyzer;


    [ExportCodeFixProvider(LanguageNames.CSharp)]
    [Shared]
    public class CancellationTokenOverloadMethodFixer :
        CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CancellationTokenOverloadMethodRuleId);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var nodeToFix = root?.FindNode(context.Span, getInnermostNodeForTie: true);
            if (nodeToFix == null)
                return;

            if (nodeToFix.IsKind(SyntaxKind.InvocationExpression))
            {
                if (!int.TryParse(context.Diagnostics[0].Properties[ParameterIndex], NumberStyles.None,
                        CultureInfo.InvariantCulture, out var parameterIndex))
                    return;

                if (!context.Diagnostics[0].Properties.TryGetValue(ParameterName, out var parameterName)
                    || parameterName == null)
                    return;

                if (!context.Diagnostics[0].Properties.TryGetValue(CancellationTokens, out var cancellationTokens)
                    || cancellationTokens == null)
                    return;

                foreach (var cancellationToken in cancellationTokens.Split(','))
                {
                    var title = $"Forward the '{cancellationToken}' parameter to the methods";
                    var codeAction = CodeAction.Create(
                        title,
                        ct => FixInvocation(context.Document, (InvocationExpressionSyntax)nodeToFix, parameterIndex, parameterName, cancellationToken, ct),
                        title);

                    context.RegisterCodeFix(codeAction, context.Diagnostics);
                }
            }
        }

        static async Task<Document> FixInvocation(Document document, InvocationExpressionSyntax nodeToFix, int index, string parameterName,
            string cancellationTokenExpression, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
            var generator = editor.Generator;

            var expression = SyntaxFactory.ParseExpression(cancellationTokenExpression);

            if (index > nodeToFix.ArgumentList.Arguments.Count)
            {
                SeparatedSyntaxList<ArgumentSyntax> newArguments =
                    nodeToFix.ArgumentList.Arguments.Add((ArgumentSyntax)generator.Argument(parameterName, RefKind.None, expression));
                editor.ReplaceNode(nodeToFix.ArgumentList, nodeToFix.ArgumentList.WithArguments(newArguments));
            }
            else
            {
                SeparatedSyntaxList<ArgumentSyntax> newArguments =
                    nodeToFix.ArgumentList.Arguments.Insert(index, (ArgumentSyntax)generator.Argument(expression));
                editor.ReplaceNode(nodeToFix.ArgumentList, nodeToFix.ArgumentList.WithArguments(newArguments));
            }

            return editor.GetChangedDocument();
        }
    }
}
