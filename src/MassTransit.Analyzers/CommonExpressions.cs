namespace MassTransit.Analyzers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;


    public static class CommonExpressions
    {
        static readonly IReadOnlyDictionary<string, int> _producerMethods = InitializeProducerMethods();
        static readonly string _taskNamespace = typeof(Task).Namespace;

        static IReadOnlyDictionary<string, int> InitializeProducerMethods()
        {
            return new Dictionary<string, int>
            {
                {"MassTransit.ISendEndpoint.Send", 0},
                {"MassTransit.IPublishEndpoint.Publish", 0},
                {"MassTransit.ConsumeContext.RespondAsync", 0},
                {"MassTransit.ConsumeContextExtensions.Forward", 0},
                {"MassTransit.ConsumeContextSelfSchedulerExtensions.ScheduleSend", 0},
                {"MassTransit.EndpointConventionExtensions.Send", 0},
                {"MassTransit.IRequestClient.Create", -1},
                {"MassTransit.IRequestClient.Request", 0},
                {"MassTransit.IRequestClient.GetResponse", -1},
                {"MassTransit.MessageInitializerExtensions.Init", 0},
                {"MassTransit.Initializers.MessageInitializerCache.Initialize", 0},
                {"MassTransit.Initializers.MessageInitializerCache.InitializeMessage", 0},
                {"MassTransit.PublishContextExecuteExtensions.Publish", 0},
                {"MassTransit.RequestHandle.GetResponse", -1},
                {"MassTransit.PublishEndpointRecurringSchedulerExtensions.ScheduleRecurringSend", 0},
                {"MassTransit.PublishEndpointSchedulerExtensions.ScheduleSend", 0},
                {"MassTransit.RedeliverExtensions.Redeliver", 0},
                {"MassTransit.RequestClientExtensions.Request", -1},
                {"MassTransit.RequestExtensions.Request", 0},
                {"MassTransit.RespondAsyncExecuteExtensions.RespondAsync", 0},
                {"MassTransit.SendContextExecuteExtensions.Send", 0},
                {"MassTransit.SendEndpointExtensions.Send", 0},
                {"MassTransit.SendEndpointRecurringSchedulerExtensions.ScheduleRecurringSend", 0},
                {"MassTransit.SendEndpointSchedulerExtensions.ScheduleSend", 0},
                {"MassTransit.TimeSpanContextScheduleExtensions.ScheduleSend", 0},
                {"MassTransit.TimeSpanScheduleExtensions.ScheduleSend", 0}
            };
        }

        public static bool IsProducerMethod(this IMethodSymbol method, out int index)
        {
            return _producerMethods.TryGetValue($"{method.ContainingNamespace}.{method.ContainingType.Name}.{method.Name}", out index);
        }

        public static bool IsActivator(this ArgumentSyntax argumentSyntax, SemanticModel semanticModel, out ITypeSymbol typeArgument)
        {
            if (argumentSyntax != null
                && argumentSyntax.Parent is ArgumentListSyntax argumentListSyntax
                && argumentListSyntax.Parent is InvocationExpressionSyntax invocationExpressionSyntax
                && invocationExpressionSyntax.Expression is MemberAccessExpressionSyntax memberAccessExpressionSyntax
                && semanticModel.GetSymbolInfo(memberAccessExpressionSyntax).Symbol is IMethodSymbol method
                && IsProducerMethod(method, out var index)
                && method.Parameters[0].Type.SpecialType == SpecialType.System_Object)
            {
                if (index == 0 && method.TypeArguments.Length == 1)
                {
                    typeArgument = method.TypeArguments[0];
                    return true;
                }

                if (index == -1 && method.ContainingType.IsGenericType && method.ContainingType.TypeArguments.Length == 1)
                {
                    typeArgument = method.ContainingType.TypeArguments[0];
                    return true;
                }
            }

            typeArgument = null;
            return false;
        }

        public static bool HasMessageContract(this ITypeSymbol typeArgument, out ITypeSymbol contractType)
        {
            if (typeArgument.TypeKind.IsClassOrInterface())
            {
                contractType = typeArgument;
                return true;
            }

            if (typeArgument.TypeKind == TypeKind.TypeParameter &&
                typeArgument is ITypeParameterSymbol typeParameter &&
                typeParameter.ConstraintTypes.Length == 1 &&
                typeParameter.ConstraintTypes[0].TypeKind.IsClassOrInterface())
            {
                contractType = typeParameter.ConstraintTypes[0];
                return true;
            }

            contractType = null;
            return false;
        }

        public static bool IsImmutableArray(this ITypeSymbol type, out ITypeSymbol typeArgument)
        {
            if (type.TypeKind == TypeKind.Struct &&
                type.Name == "ImmutableArray" &&
                type.ContainingNamespace.ToString() == "System.Collections.Immutable" &&
                type is INamedTypeSymbol immutableArrayType &&
                immutableArrayType.IsGenericType &&
                immutableArrayType.TypeArguments.Length == 1)
            {
                typeArgument = immutableArrayType.TypeArguments[0];
                return true;
            }

            typeArgument = null;
            return false;
        }

        public static bool IsCollection(this ITypeSymbol type, out ITypeSymbol typeArgument)
        {
            if (type.TypeKind == TypeKind.Interface &&
                type.Name == "ICollection" &&
                type.ContainingNamespace.ToString() == "System.Collections.Generic" &&
                type is INamedTypeSymbol collectionType &&
                collectionType.IsGenericType &&
                collectionType.TypeArguments.Length == 1)
            {
                typeArgument = collectionType.TypeArguments[0];
                return true;
            }

            typeArgument = null;
            return false;
        }

        public static bool IsEnumerable(this ITypeSymbol type, out ITypeSymbol typeArgument)
        {
            if (type.TypeKind == TypeKind.Interface &&
                type.Name == "IEnumerable" &&
                type.ContainingNamespace.ToString() == "System.Collections.Generic" &&
                type is INamedTypeSymbol collectionType &&
                collectionType.IsGenericType &&
                collectionType.TypeArguments.Length == 1)
            {
                typeArgument = collectionType.TypeArguments[0];
                return true;
            }

            typeArgument = null;
            return false;
        }

        public static bool IsList(this ITypeSymbol type, out ITypeSymbol typeArgument)
        {
            if ((type.TypeKind == TypeKind.Class && type.Name == "List"
                    || type.TypeKind.IsClassOrInterface() && type.Name == "IReadOnlyList"
                    || type.TypeKind.IsClassOrInterface() && type.Name == "IList")
                && type.ContainingNamespace.ToString() == "System.Collections.Generic"
                && type is INamedTypeSymbol listType
                && listType.IsGenericType
                && listType.TypeArguments.Length == 1)
            {
                typeArgument = listType.TypeArguments[0];
                return true;
            }

            typeArgument = null;
            return false;
        }

        public static List<IPropertySymbol> GetContractProperties(this ITypeSymbol contractType)
        {
            var contractTypes = new List<ITypeSymbol> {contractType};

            contractTypes.AddRange(contractType.AllInterfaces);

            #pragma warning disable RS1024
            return contractTypes.SelectMany(i => i.GetMembers().OfType<IPropertySymbol>().Where(x => x.DeclaredAccessibility == Accessibility.Public))
                .Distinct(PropertyNameEqualityComparer.Instance)
                .ToList();
        }

        public static bool IsDictionary(this ITypeSymbol type, out ITypeSymbol keyType, out ITypeSymbol valueType)
        {
            if ((type.TypeKind == TypeKind.Class && type.Name == "Dictionary"
                    || type.TypeKind.IsClassOrInterface() && type.Name == "IReadOnlyDictionary"
                    || type.TypeKind.IsClassOrInterface() && type.Name == "IDictionary")
                && type.ContainingNamespace.ToString() == "System.Collections.Generic"
                && type is INamedTypeSymbol dictionaryType
                && dictionaryType.IsGenericType
                && dictionaryType.TypeArguments.Length == 2)
            {
                keyType = dictionaryType.TypeArguments[0];
                valueType = dictionaryType.TypeArguments[1];
                return true;
            }

            keyType = null;
            valueType = null;
            return false;
        }

        public static bool IsNullable(this ITypeSymbol type, out ITypeSymbol typeArgument)
        {
            if (type.TypeKind == TypeKind.Struct &&
                type.Name == "Nullable" &&
                type.ContainingNamespace.Name == "System" &&
                type is INamedTypeSymbol nullableType &&
                nullableType.IsGenericType &&
                nullableType.TypeArguments.Length == 1)
            {
                typeArgument = nullableType.TypeArguments[0];
                return true;
            }

            typeArgument = null;
            return false;
        }

        public static bool IsArray(this ITypeSymbol type, out ITypeSymbol elementType)
        {
            if (type.TypeKind == TypeKind.Array &&
                type is IArrayTypeSymbol arrayTypeSymbol)
            {
                elementType = arrayTypeSymbol.ElementType;
                return true;
            }

            elementType = null;
            return false;
        }

        public static bool IsInVar(this ITypeSymbol type, out ITypeSymbol inVarType)
        {
            if (type.TypeKind == TypeKind.Class
                && type.ContainingNamespace.Name == "Variables"
                && type.ContainingNamespace.ContainingNamespace.Name == "Initializers"
                && type.ContainingNamespace.ContainingNamespace.ContainingNamespace.Name == "MassTransit")
            {
                var inVar = type.Interfaces.FirstOrDefault(i => i.Name == "IInitializerVariable");
                if (inVar != null &&
                    inVar.IsGenericType &&
                    inVar.TypeArguments.Length == 1)
                {
                    inVarType = inVar.TypeArguments[0];
                    return true;
                }
            }

            inVarType = null;
            return false;
        }

        public static bool ReturnsTask(this IMethodSymbol method)
        {
            return method.ReturnType.Name == nameof(Task) && method.ReturnType.ContainingNamespace.ToString() == _taskNamespace;
        }

        public static IEnumerable<INamedTypeSymbol> GetAllInterfaces(this ITypeSymbol type)
        {
            ImmutableArray<INamedTypeSymbol> allInterfaces = type.AllInterfaces;
            if (type is INamedTypeSymbol namedType && namedType.TypeKind.IsClassOrInterface() && !allInterfaces.Contains(namedType))
            {
                var result = new List<INamedTypeSymbol>(allInterfaces.Length + 1) {namedType};
                result.AddRange(allInterfaces);
                return result;
            }

            return allInterfaces;
        }

        /// <summary>
        /// Return the type, and any base types
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<ITypeSymbol> GetAllTypes(this ITypeSymbol type)
        {
            var current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static bool IsClassOrInterface(this TypeKind typeKind)
        {
            return typeKind == TypeKind.Interface || typeKind == TypeKind.Class;
        }

        public static bool ImplementsInterface(this ITypeSymbol symbol, ITypeSymbol type)
        {
            return symbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, type));
        }

        public static bool InheritsFromType(this ITypeSymbol symbol, ITypeSymbol type)
        {
            return GetAllTypes(symbol).Any(x => SymbolEqualityComparer.Default.Equals(x, type));
        }

        public static bool ImplementsType(this ITypeSymbol type, ITypeSymbol otherType)
        {
            IEnumerable<ITypeSymbol> types = GetAllTypes(type);
            IEnumerable<INamedTypeSymbol> interfaces = GetAllInterfaces(type);

            return types.Any(baseType => SymbolEqualityComparer.Default.Equals(baseType, otherType))
                || interfaces.Any(baseInterfaceType => SymbolEqualityComparer.Default.Equals(baseInterfaceType, otherType));
        }
    }
}
