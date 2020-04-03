namespace MassTransit.Analyzers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Helpers;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;


    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MessageContractAnalyzer :
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

        readonly ConcurrentDictionary<SemanticModel, Lazy<TypeConversionHelper>> _typeConverterHelpers =
            new ConcurrentDictionary<SemanticModel, Lazy<TypeConversionHelper>>();

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
            context.RegisterSyntaxNodeAction(AnalyzeAnonymousObjectCreationNode, SyntaxKind.AnonymousObjectCreationExpression);
        }

        void AnalyzeAnonymousObjectCreationNode(SyntaxNodeAnalysisContext context)
        {
            var typeConverterHelper = GetTypeConverterHelper(context);

            var anonymousObject = (AnonymousObjectCreationExpressionSyntax)context.Node;

            if (anonymousObject.Parent is ArgumentSyntax argumentSyntax
                && argumentSyntax.IsActivator(context.SemanticModel, out var typeArgument))
            {
                if (typeArgument.HasMessageContract(out var messageContractType))
                {
                    var anonymousType = context.SemanticModel.GetTypeInfo(anonymousObject).Type;

                    var incompatibleProperties = new List<string>();
                    if (!TypesAreStructurallyCompatible(typeConverterHelper, messageContractType, anonymousType, string.Empty, incompatibleProperties))
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

        TypeConversionHelper GetTypeConverterHelper(SyntaxNodeAnalysisContext context)
        {
            return _typeConverterHelpers.GetOrAdd(context.SemanticModel, model => new Lazy<TypeConversionHelper>(() => new TypeConversionHelper(model))).Value;
        }

        static bool TypesAreStructurallyCompatible(TypeConversionHelper typeConverterHelper, ITypeSymbol contractType, ITypeSymbol inputType,
            string path, ICollection<string> incompatibleProperties)
        {
            if (SymbolEqualityComparer.Default.Equals(inputType, contractType))
                return true;

            List<IPropertySymbol> contractProperties = GetContractProperties(contractType);
            List<IPropertySymbol> inputProperties = GetInputProperties(inputType);
            var result = true;

            foreach (var inputProperty in inputProperties)
            {
                var contractProperty = contractProperties.FirstOrDefault(m => m.Name == inputProperty.Name);

                if (contractProperty == null)
                {
                    if (!IsHeaderProperty(typeConverterHelper, inputProperty))
                    {
                        incompatibleProperties.Add(Append(path, inputProperty.Name));
                        result = false;
                    }
                }
                else if (!typeConverterHelper.CanConvert(contractProperty.Type, inputProperty.Type))
                {
                    if (inputProperty.Type.IsAnonymousType)
                    {
                        if (contractProperty.Type.TypeKind == TypeKind.Interface)
                        {
                            if (!TypesAreStructurallyCompatible(typeConverterHelper, contractProperty.Type,
                                inputProperty.Type, Append(path, inputProperty.Name), incompatibleProperties))
                                result = false;
                        }
                        else
                        {
                            incompatibleProperties.Add(Append(path, inputProperty.Name));
                            result = false;
                        }
                    }
                    else if (contractProperty.Type.IsImmutableArray(out var contractElementType) ||
                        contractProperty.Type.IsList(out contractElementType) ||
                        contractProperty.Type.IsArray(out contractElementType))
                    {
                        if (inputProperty.Type.IsImmutableArray(out var inputElementType) ||
                            inputProperty.Type.IsList(out inputElementType) ||
                            inputProperty.Type.IsArray(out inputElementType))
                        {
                            if (!typeConverterHelper.CanConvert(contractElementType, inputElementType))
                            {
                                if (contractElementType.TypeKind == TypeKind.Interface)
                                {
                                    if (!TypesAreStructurallyCompatible(typeConverterHelper, contractElementType,
                                        inputElementType, Append(path, inputProperty.Name), incompatibleProperties))
                                        result = false;
                                }
                                else
                                {
                                    incompatibleProperties.Add(Append(path, inputProperty.Name));
                                    result = false;
                                }
                            }
                        }
                        // a single element will be added to a list in the message contract
                        else if (!typeConverterHelper.CanConvert(contractElementType, inputProperty.Type))
                        {
                            incompatibleProperties.Add(Append(path, inputProperty.Name));
                            result = false;
                        }
                    }
                    else if (contractProperty.Type.IsDictionary(out var contractKeyType, out var contractValueType))
                    {
                        if (inputProperty.Type.IsDictionary(out var inputKeyType, out var inputValueType))
                        {
                            if (typeConverterHelper.CanConvert(contractKeyType, inputKeyType))
                            {
                                if (!typeConverterHelper.CanConvert(contractValueType, inputValueType))
                                {
                                    if (contractValueType.TypeKind == TypeKind.Interface)
                                    {
                                        if (!TypesAreStructurallyCompatible(typeConverterHelper, contractValueType,
                                            inputValueType, Append(path, inputProperty.Name), incompatibleProperties))
                                            result = false;
                                    }
                                    else
                                    {
                                        incompatibleProperties.Add(Append(path, inputProperty.Name));
                                        result = false;
                                    }
                                }
                            }
                            else
                            {
                                incompatibleProperties.Add(Append(path, inputProperty.Name));
                                result = false;
                            }
                        }
                        else
                        {
                            incompatibleProperties.Add(Append(path, inputProperty.Name));
                            result = false;
                        }
                    }
                    else
                    {
                        incompatibleProperties.Add(Append(path, inputProperty.Name));
                        result = false;
                    }
                }
            }

            return result;
        }

        static bool IsHeaderProperty(TypeConversionHelper typeConverterHelper, IPropertySymbol messageProperty)
        {
            if (!messageProperty.Name.StartsWith("__"))
                return false;

            if (messageProperty.Name.StartsWith("__Header_"))
                return true;

            return messageProperty.Name switch
            {
                "__SourceAddress" => typeConverterHelper.CanConvert(typeof(Uri), messageProperty.Type),
                "__DestinationAddress" => typeConverterHelper.CanConvert(typeof(Uri), messageProperty.Type),
                "__ResponseAddress" => typeConverterHelper.CanConvert(typeof(Uri), messageProperty.Type),
                "__FaultAddress" => typeConverterHelper.CanConvert(typeof(Uri), messageProperty.Type),
                "__RequestId" => typeConverterHelper.CanConvert(typeof(Guid), messageProperty.Type),
                "__MessageId" => typeConverterHelper.CanConvert(typeof(Guid), messageProperty.Type),
                "__ConversationId" => typeConverterHelper.CanConvert(typeof(Guid), messageProperty.Type),
                "__CorrelationId" => typeConverterHelper.CanConvert(typeof(Guid), messageProperty.Type),
                "__InitiatorId" => typeConverterHelper.CanConvert(typeof(Guid), messageProperty.Type),
                "__ScheduledMessageId" => typeConverterHelper.CanConvert(typeof(Guid), messageProperty.Type),
                "__TimeToLive" => typeConverterHelper.CanConvert(typeof(TimeSpan), messageProperty.Type),
                "__Durable" => typeConverterHelper.CanConvert(typeof(bool), messageProperty.Type),
                _ => false
            };
        }

        static bool HasMissingProperties(ITypeSymbol inputType, ITypeSymbol contractType, string path, ICollection<string> missingProperties)
        {
            List<IPropertySymbol> contractProperties = GetContractProperties(contractType);
            List<IPropertySymbol> inputProperties = GetInputProperties(inputType);
            var result = false;

            foreach (var contractProperty in contractProperties)
            {
                var inputProperty = inputProperties.FirstOrDefault(m => m.Name == contractProperty.Name);

                if (inputProperty == null)
                {
                    missingProperties.Add(Append(path, contractProperty.Name));
                    result = true;
                }
                else if (contractProperty.Type.IsImmutableArray(out var contractElementType) ||
                    contractProperty.Type.IsList(out contractElementType) ||
                    contractProperty.Type.IsArray(out contractElementType))
                {
                    if (contractElementType.TypeKind == TypeKind.Interface)
                    {
                        if (inputProperty.Type.IsImmutableArray(out var inputElementType) ||
                            inputProperty.Type.IsList(out inputElementType) ||
                            inputProperty.Type.IsArray(out inputElementType))
                        {
                            if (HasMissingProperties(inputElementType, contractElementType, Append(path, contractProperty.Name), missingProperties))
                                result = true;
                        }
                    }
                }
                else if (contractProperty.Type.TypeKind == TypeKind.Interface)
                {
                    if (inputProperty.Type.IsAnonymousType)
                    {
                        if (HasMissingProperties(inputProperty.Type, contractProperty.Type, Append(path, contractProperty.Name), missingProperties))
                            result = true;
                    }
                }
            }

            return result;
        }

        static List<IPropertySymbol> GetInputProperties(ITypeSymbol inputType)
        {
            return inputType.GetMembers().OfType<IPropertySymbol>().ToList();
        }

        static List<IPropertySymbol> GetContractProperties(ITypeSymbol contractType)
        {
            var contractTypes = new List<ITypeSymbol> {contractType};

            contractTypes.AddRange(contractType.AllInterfaces);

            return contractTypes.SelectMany(i => i.GetMembers().OfType<IPropertySymbol>()).Distinct(PropertyNameEqualityComparer.Instance).ToList();
        }

        static string Append(string path, string propertyName)
        {
            if (string.IsNullOrEmpty(path))
                return propertyName;

            if (path.EndsWith(".", StringComparison.Ordinal))
                return $"{path}{propertyName}";

            return $"{path}.{propertyName}";
        }


        class PropertyNameEqualityComparer :
            IEqualityComparer<IPropertySymbol>
        {
            public static readonly PropertyNameEqualityComparer Instance = new PropertyNameEqualityComparer();

            public bool Equals(IPropertySymbol x, IPropertySymbol y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;
                return x.Name.Equals(y.Name);
            }

            public int GetHashCode(IPropertySymbol obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
