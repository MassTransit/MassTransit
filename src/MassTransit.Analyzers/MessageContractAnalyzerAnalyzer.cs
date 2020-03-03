namespace MassTransit.Analyzers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;


    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MessageContractAnalyzerAnalyzer :
        DiagnosticAnalyzer
    {
        public const string StructurallyCompatibleRuleId = "MCA0001";
        public const string ValidMessageContractStructureRuleId = "MCA0002";
        public const string MissingPropertiesRuleId = "MCA0003";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization

        const string Category = "Usage";

        static readonly DiagnosticDescriptor StructurallyCompatibleRule = new DiagnosticDescriptor(StructurallyCompatibleRuleId,
            "Anonymous type does not map to message contract",
            "Anonymous type does not map to message contract '{0}'. The following properties of the anonymous type are incompatible: {1}",
            Category, DiagnosticSeverity.Error, true,
            "Anonymous type should map to message contract");

        static readonly DiagnosticDescriptor ValidMessageContractStructureRule = new DiagnosticDescriptor(ValidMessageContractStructureRuleId,
            "Message contract does not have a valid structure",
            "Message contract '{0}' does not have a valid structure",
            Category, DiagnosticSeverity.Error, true,
            "Message contract should have a valid structure. Properties should be primitive, string or IReadOnlyList or ImmutableArray of a primitive, string or message contract");

        static readonly DiagnosticDescriptor MissingPropertiesRule = new DiagnosticDescriptor(MissingPropertiesRuleId,
            "Anonymous type is missing properties that are in the message contract",
            "Anonymous type is missing properties that are in the message contract '{0}'. The following properties are missing: {1}",
            Category, DiagnosticSeverity.Info, true,
            "Anonymous type misses properties that are in the message contract");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(StructurallyCompatibleRule, ValidMessageContractStructureRule, MissingPropertiesRule);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.AnonymousObjectCreationExpression);
        }

        static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var anonymousObject = (AnonymousObjectCreationExpressionSyntax)context.Node;

            if (anonymousObject.Parent is ArgumentSyntax argumentSyntax &&
                argumentSyntax.IsActivator(context.SemanticModel, out var typeArgument))
            {
                if (typeArgument.HasMessageContract(out var messageContractType))
                {
                    var anonymousType = context.SemanticModel.GetTypeInfo(anonymousObject).Type;

                    var incompatibleProperties = new List<string>();
                    if (!TypesAreStructurallyCompatible(anonymousType, messageContractType, string.Empty, incompatibleProperties))
                    {
                        var diagnostic = Diagnostic.Create(StructurallyCompatibleRule, anonymousType.Locations[0],
                            messageContractType.Name, string.Join(", ", incompatibleProperties));
                        context.ReportDiagnostic(diagnostic);
                    }

                    var missingProperties = new List<string>();
                    if (HasMissingProperties(anonymousType, messageContractType, string.Empty, missingProperties))
                    {
                        var diagnostic = Diagnostic.Create(MissingPropertiesRule, anonymousType.Locations[0],
                            messageContractType.Name, string.Join(", ", missingProperties));
                        context.ReportDiagnostic(diagnostic);
                    }
                }
                else
                {
                    var diagnostic = Diagnostic.Create(ValidMessageContractStructureRule, context.Node.GetLocation(), typeArgument.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        static bool TypesAreStructurallyCompatible(ITypeSymbol messageType, ITypeSymbol messageContractType, string path,
            ICollection<string> incompatibleProperties)
        {
            if (SymbolEqualityComparer.Default.Equals(messageType, messageContractType))
                return true;

            List<IPropertySymbol> messageContractProperties = GetMessageContractProperties(messageContractType);
            List<IPropertySymbol> messageProperties = GetMessageProperties(messageType);
            var result = true;

            foreach (var messageProperty in messageProperties)
            {
                var messageContractProperty =
                    messageContractProperties.FirstOrDefault(m => m.Name == messageProperty.Name);

                if (messageContractProperty == null)
                {
                    incompatibleProperties.Add(Append(path, messageProperty.Name));
                    result = false;
                }
                else if (!SymbolEqualityComparer.Default.Equals(messageProperty.Type, messageContractProperty.Type)
                    && !(messageProperty.Type.IsInVar(out var inVarType) && SymbolEqualityComparer.Default.Equals(inVarType, messageContractProperty.Type)))
                {
                    if (messageProperty.Type.IsAnonymousType)
                    {
                        if (messageContractProperty.Type.TypeKind == TypeKind.Interface)
                        {
                            if (!TypesAreStructurallyCompatible(messageProperty.Type, messageContractProperty.Type,
                                Append(path, messageProperty.Name), incompatibleProperties))
                                result = false;
                        }
                        else
                        {
                            incompatibleProperties.Add(Append(path, messageProperty.Name));
                            result = false;
                        }
                    }
                    else if (messageProperty.Type.IsImmutableArray(out var messagePropertyTypeArgument) ||
                        messageProperty.Type.IsReadOnlyList(out messagePropertyTypeArgument) ||
                        messageProperty.Type.IsList(out messagePropertyTypeArgument) ||
                        messageProperty.Type.IsArray(out messagePropertyTypeArgument))
                    {
                        if (messageContractProperty.Type.IsImmutableArray(out var messageContractPropertyTypeArgument) ||
                            messageContractProperty.Type.IsReadOnlyList(out messageContractPropertyTypeArgument) ||
                            messageContractProperty.Type.IsArray(out messageContractPropertyTypeArgument))
                        {
                            if (!SymbolEqualityComparer.Default.Equals(messagePropertyTypeArgument, messageContractPropertyTypeArgument))
                            {
                                if (messageContractPropertyTypeArgument.TypeKind == TypeKind.Interface)
                                {
                                    if (!TypesAreStructurallyCompatible(messagePropertyTypeArgument, messageContractPropertyTypeArgument,
                                        Append(path, messageProperty.Name), incompatibleProperties))
                                        result = false;
                                }
                                else
                                {
                                    incompatibleProperties.Add(Append(path, messageProperty.Name));
                                    result = false;
                                }
                            }
                        }
                        else
                        {
                            incompatibleProperties.Add(Append(path, messageProperty.Name));
                            result = false;
                        }
                    }
                    else
                    {
                        incompatibleProperties.Add(Append(path, messageProperty.Name));
                        result = false;
                    }
                }
            }

            return result;
        }

        static bool HasMissingProperties(ITypeSymbol messageType, ITypeSymbol messageContractType, string path, ICollection<string> missingProperties)
        {
            List<IPropertySymbol> messageContractProperties = GetMessageContractProperties(messageContractType);
            List<IPropertySymbol> messageProperties = GetMessageProperties(messageType);
            var result = false;

            foreach (var messageContractProperty in messageContractProperties)
            {
                var messageProperty =
                    messageProperties.FirstOrDefault(m => m.Name == messageContractProperty.Name);

                if (messageProperty == null)
                {
                    missingProperties.Add(Append(path, messageContractProperty.Name));
                    result = true;
                }
                else if (messageContractProperty.Type.IsImmutableArray(out var messageContractPropertyTypeArgument) ||
                    messageContractProperty.Type.IsReadOnlyList(out messageContractPropertyTypeArgument) ||
                    messageContractProperty.Type.IsArray(out messageContractPropertyTypeArgument))
                {
                    if (messageContractPropertyTypeArgument.TypeKind == TypeKind.Interface)
                    {
                        if (messageProperty.Type.IsImmutableArray(out var messagePropertyTypeArgument) ||
                            messageProperty.Type.IsReadOnlyList(out messagePropertyTypeArgument) ||
                            messageProperty.Type.IsList(out messagePropertyTypeArgument) ||
                            messageProperty.Type.IsArray(out messagePropertyTypeArgument))
                        {
                            var hasMissingProperties = HasMissingProperties(messagePropertyTypeArgument, messageContractPropertyTypeArgument,
                                Append(path, messageContractProperty.Name), missingProperties);
                            if (hasMissingProperties)
                                result = true;
                        }
                    }
                }
                else if (messageContractProperty.Type.TypeKind == TypeKind.Interface)
                {
                    if (messageProperty.Type.IsAnonymousType)
                    {
                        var hasMissingProperties = HasMissingProperties(messageProperty.Type, messageContractProperty.Type,
                            Append(path, messageContractProperty.Name), missingProperties);
                        if (hasMissingProperties)
                            result = true;
                    }
                }
            }

            return result;
        }

        static List<IPropertySymbol> GetMessageProperties(ITypeSymbol messageType)
        {
            return messageType.GetMembers().OfType<IPropertySymbol>().Where(p => !p.Name.StartsWith("__")).ToList();
        }

        static List<IPropertySymbol> GetMessageContractProperties(ITypeSymbol messageContractType)
        {
            var messageContractTypes = new List<ITypeSymbol> {messageContractType};
            messageContractTypes.AddRange(messageContractType.AllInterfaces);

            return messageContractTypes.SelectMany(i => i.GetMembers().OfType<IPropertySymbol>()).ToList();
        }

        static string Append(string path, string propertyName)
        {
            if (string.IsNullOrEmpty(path))
                return propertyName;

            if (path.EndsWith(".", StringComparison.Ordinal))
                return $"{path}{propertyName}";

            return $"{path}.{propertyName}";
        }
    }
}
