namespace MassTransit.Analyzers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Simplification;


    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MessageContractAnalyzerCodeFixProvider))]
    [Shared]
    public class MessageContractAnalyzerCodeFixProvider : CodeFixProvider
    {
        const string Title = "Add missing properties";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MessageContractAnalyzerAnalyzer.MissingPropertiesRuleId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var anonymousObject = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<AnonymousObjectCreationExpressionSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    Title,
                    cancellationToken => AddMissingProperties(context.Document, anonymousObject, cancellationToken),
                    Title),
                diagnostic);
        }

        static async Task<Document> AddMissingProperties(Document document,
            AnonymousObjectCreationExpressionSyntax anonymousObject, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            if (anonymousObject.Parent is ArgumentSyntax argumentSyntax &&
                argumentSyntax.IsActivator(semanticModel, out var typeArgument) &&
                typeArgument.HasMessageContract(out var messageContractType))
            {
                var dictionary = new Dictionary<AnonymousObjectCreationExpressionSyntax, ITypeSymbol>();

                await FindAnonymousTypesWithMessageContractsInTree(dictionary, anonymousObject, messageContractType, semanticModel).ConfigureAwait(false);

                var newRoot = AddMissingProperties(root, dictionary);

                var formattedRoot = Formatter.Format(newRoot, Formatter.Annotation, document.Project.Solution.Workspace,
                    document.Project.Solution.Workspace.Options);

                return document.WithSyntaxRoot(formattedRoot);
            }

            return document;
        }

        static async Task FindAnonymousTypesWithMessageContractsInTree(IDictionary<AnonymousObjectCreationExpressionSyntax, ITypeSymbol> dictionary,
            AnonymousObjectCreationExpressionSyntax anonymousObject, ITypeSymbol messageContractType, SemanticModel semanticModel)
        {
            List<IPropertySymbol> messageContractProperties = GetMessageContractProperties(messageContractType);

            foreach (var initializer in anonymousObject.Initializers)
            {
                var name = GetName(initializer);

                var messageContractProperty = messageContractProperties.FirstOrDefault(p => p.Name == name);

                if (messageContractProperty != null)
                {
                    if (initializer.Expression is ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpressionSyntax)
                    {
                        if (messageContractProperty.Type.IsImmutableArray(out var messageContractPropertyTypeArgument) ||
                            messageContractProperty.Type.IsReadOnlyList(out messageContractPropertyTypeArgument) ||
                            messageContractProperty.Type.IsArray(out messageContractPropertyTypeArgument))
                        {
                            SeparatedSyntaxList<ExpressionSyntax> expressions = implicitArrayCreationExpressionSyntax.Initializer.Expressions;
                            foreach (var expression in expressions)
                            {
                                if (expression is AnonymousObjectCreationExpressionSyntax anonymousObjectArrayInitializer)
                                    await FindAnonymousTypesWithMessageContractsInTree(dictionary, anonymousObjectArrayInitializer,
                                        messageContractPropertyTypeArgument, semanticModel).ConfigureAwait(false);
                            }
                        }
                    }
                    else if (initializer.Expression is AnonymousObjectCreationExpressionSyntax anonymousObjectProperty)
                        await FindAnonymousTypesWithMessageContractsInTree(dictionary, anonymousObjectProperty, messageContractProperty.Type, semanticModel)
                            .ConfigureAwait(false);
                    else if (initializer.Expression is InvocationExpressionSyntax invocationExpressionSyntax
                        && semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is IMethodSymbol method
                        && method.ReturnType.IsList(out var methodReturnTypeArgument)
                        && methodReturnTypeArgument.IsAnonymousType)
                    {
                        if (messageContractProperty.Type.IsImmutableArray(out var messageContractPropertyTypeArgument) ||
                            messageContractProperty.Type.IsReadOnlyList(out messageContractPropertyTypeArgument) ||
                            messageContractProperty.Type.IsArray(out messageContractPropertyTypeArgument))
                        {
                            var syntax = await methodReturnTypeArgument.DeclaringSyntaxReferences[0].GetSyntaxAsync().ConfigureAwait(false);
                            if (syntax is AnonymousObjectCreationExpressionSyntax anonymousObjectTypeArgument)
                                await FindAnonymousTypesWithMessageContractsInTree(dictionary, anonymousObjectTypeArgument, messageContractPropertyTypeArgument,
                                    semanticModel).ConfigureAwait(false);
                        }
                    }
                }
            }

            dictionary.Add(anonymousObject, messageContractType);
        }

        static string GetName(AnonymousObjectMemberDeclaratorSyntax initializer)
        {
            string name;
            if (initializer.NameEquals == null)
            {
                var expression = (MemberAccessExpressionSyntax)initializer.Expression;
                name = expression.Name.Identifier.Text;
            }
            else
                name = initializer.NameEquals.Name.Identifier.Text;

            return name;
        }

        static List<IPropertySymbol> GetMessageContractProperties(ITypeSymbol messageContractType)
        {
            var messageContractTypes = new List<ITypeSymbol> {messageContractType};
            messageContractTypes.AddRange(messageContractType.AllInterfaces);

            return messageContractTypes.SelectMany(i => i.GetMembers().OfType<IPropertySymbol>()).ToList();
        }

        static SyntaxNode AddMissingProperties(SyntaxNode root, IDictionary<AnonymousObjectCreationExpressionSyntax, ITypeSymbol> dictionary)
        {
            var newRoot = root.TrackNodes(dictionary.Keys);

            foreach (KeyValuePair<AnonymousObjectCreationExpressionSyntax, ITypeSymbol> keyValuePair in dictionary)
            {
                var anonymousObject = newRoot.GetCurrentNode(keyValuePair.Key);
                var messageContractType = keyValuePair.Value;
                newRoot = AddMissingProperties(newRoot, anonymousObject, messageContractType);
            }

            return newRoot;
        }

        static SyntaxNode AddMissingProperties(SyntaxNode root,
            AnonymousObjectCreationExpressionSyntax anonymousObject, ITypeSymbol messageContractType)
        {
            var newRoot = root;

            List<IPropertySymbol> messageContractProperties = GetMessageContractProperties(messageContractType);

            var propertiesToAdd = new List<AnonymousObjectMemberDeclaratorSyntax>();
            foreach (var messageContractProperty in messageContractProperties)
            {
                var initializer =
                    anonymousObject.Initializers.FirstOrDefault(i => GetName(i) == messageContractProperty.Name);

                if (initializer == null)
                {
                    var propertyToAdd = CreateProperty(messageContractProperty);
                    propertiesToAdd.Add(propertyToAdd);
                }
            }

            if (propertiesToAdd.Any())
            {
                var newAnonymousObject = anonymousObject
                    .AddInitializers(propertiesToAdd.ToArray())
                    .WithAdditionalAnnotations(Formatter.Annotation);
                newRoot = newRoot.ReplaceNode(anonymousObject, newAnonymousObject);
            }

            return newRoot;
        }

        static AnonymousObjectMemberDeclaratorSyntax[] CreateProperties(ITypeSymbol messageContractType)
        {
            List<IPropertySymbol> messageContractProperties = GetMessageContractProperties(messageContractType);

            var propertiesToAdd = new List<AnonymousObjectMemberDeclaratorSyntax>();
            foreach (var messageContractProperty in messageContractProperties)
            {
                var propertyToAdd = CreateProperty(messageContractProperty);
                propertiesToAdd.Add(propertyToAdd);
            }

            return propertiesToAdd.ToArray();
        }

        static AnonymousObjectMemberDeclaratorSyntax CreateProperty(IPropertySymbol messageContractProperty)
        {
            ExpressionSyntax expression;
            if (messageContractProperty.Type.IsImmutableArray(out var messageContractPropertyTypeArgument) ||
                messageContractProperty.Type.IsReadOnlyList(out messageContractPropertyTypeArgument) ||
                messageContractProperty.Type.IsArray(out messageContractPropertyTypeArgument))
                expression = CreateImplicitArray(messageContractPropertyTypeArgument);
            else if (messageContractProperty.Type.TypeKind == TypeKind.Interface)
                expression = CreateAnonymousObject(messageContractProperty.Type);
            else if (messageContractProperty.Type.IsNullable(out var nullableTypeArgument))
                expression = CreateDefaultNullable(nullableTypeArgument);
            else
                expression = CreateDefault(messageContractProperty.Type);

            return SyntaxFactory.AnonymousObjectMemberDeclarator(SyntaxFactory.NameEquals(messageContractProperty.Name), expression)
                .WithAdditionalAnnotations(Formatter.Annotation)
                .WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed);
        }

        static ImplicitArrayCreationExpressionSyntax CreateImplicitArray(ITypeSymbol type)
        {
            ExpressionSyntax node;
            if (type.TypeKind == TypeKind.Interface)
                node = CreateAnonymousObject(type);
            else
                node = CreateDefault(type);

            var nodes = new[] {node};
            var initializer = SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression)
                .WithExpressions(SyntaxFactory.SeparatedList(nodes));
            return SyntaxFactory.ImplicitArrayCreationExpression(initializer)
                .WithAdditionalAnnotations(Formatter.Annotation);
        }

        static AnonymousObjectCreationExpressionSyntax CreateAnonymousObject(ITypeSymbol type)
        {
            AnonymousObjectMemberDeclaratorSyntax[] propertiesToAdd = CreateProperties(type);
            return SyntaxFactory.AnonymousObjectCreationExpression()
                .WithInitializers(SyntaxFactory.SeparatedList(propertiesToAdd))
                .WithAdditionalAnnotations(Formatter.Annotation);
        }

        static DefaultExpressionSyntax CreateDefault(ITypeSymbol type)
        {
            return SyntaxFactory.DefaultExpression(SyntaxFactory.ParseTypeName(type.Name).WithAdditionalAnnotations(Simplifier.Annotation));
        }

        static DefaultExpressionSyntax CreateDefaultNullable(ITypeSymbol type)
        {
            return SyntaxFactory.DefaultExpression(
                SyntaxFactory.NullableType(SyntaxFactory.ParseTypeName(type.Name).WithAdditionalAnnotations(Simplifier.Annotation)));
        }
    }
}
