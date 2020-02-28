using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace MassTransit.Analyzers
{
    public static class CommonExpressions
    {
        public static bool IsActivator(this ArgumentSyntax argumentSyntax, SemanticModel semanticModel, out ITypeSymbol typeArgument)
        {
            if (argumentSyntax != null
                && argumentSyntax.Parent is ArgumentListSyntax argumentListSyntax
                && argumentListSyntax.Parent is InvocationExpressionSyntax invocationExpressionSyntax
                && invocationExpressionSyntax.Expression is MemberAccessExpressionSyntax memberAccessExpressionSyntax
                && semanticModel.GetSymbolInfo(memberAccessExpressionSyntax).Symbol is IMethodSymbol method
                && method.Parameters[0].Type.SpecialType == SpecialType.System_Object)
            {
                if (method.TypeArguments.Length == 1
                    && IsGenericInitializerMethod(method))
                {
                    typeArgument = method.TypeArguments[0];
                    return true;
                }

                if (method.ContainingType.IsGenericType
                    && method.ContainingType.TypeArguments.Length == 1
                    && IsInitializerMethod(method))
                {
                    typeArgument = method.ContainingType.TypeArguments[0];
                    return true;
                }
            }

            typeArgument = null;
            return false;
        }

        static bool IsGenericInitializerMethod(IMethodSymbol method)
        {
            return method != null
                   && method.ContainingType.ContainingAssembly.Name == "MassTransit"
                   && (method.Name == "Publish" && method.ContainingType.Name == "IPublishEndpoint"
                       || method.Name == "Send" && method.ContainingType.Name == "ISendEndpoint"
                       || method.Name == "Create" && method.ContainingType.Name == "IRequestClient"
                       || method.Name == "RespondAsync" && method.ContainingType.Name == "ConsumeContext");
        }

        static bool IsInitializerMethod(IMethodSymbol method)
        {
            return method != null
                   && method.ContainingType.ContainingAssembly.Name == "MassTransit"
                   && method.Name == "Create" && method.ContainingType.Name == "IRequestClient";
        }

        public static bool HasMessageContract(this ITypeSymbol typeArgument, out ITypeSymbol messageContractType)
        {
            if (typeArgument.TypeKind == TypeKind.Interface)
            {
                messageContractType = typeArgument;
                return true;
            }

            if (typeArgument.TypeKind == TypeKind.TypeParameter &&
                typeArgument is ITypeParameterSymbol typeParameter &&
                typeParameter.ConstraintTypes.Length == 1 &&
                typeParameter.ConstraintTypes[0].TypeKind == TypeKind.Interface)
            {
                messageContractType = typeParameter.ConstraintTypes[0];
                return true;
            }

            messageContractType = null;
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

        public static bool IsReadOnlyList(this ITypeSymbol type, out ITypeSymbol typeArgument)
        {
            if (type.TypeKind == TypeKind.Interface &&
                type.Name == "IReadOnlyList" &&
                type.ContainingNamespace.ToString() == "System.Collections.Generic" &&
                type is INamedTypeSymbol readOnlyListType &&
                readOnlyListType.IsGenericType &&
                readOnlyListType.TypeArguments.Length == 1)
            {
                typeArgument = readOnlyListType.TypeArguments[0];
                return true;
            }

            typeArgument = null;
            return false;
        }

        public static bool IsList(this ITypeSymbol type, out ITypeSymbol typeArgument)
        {
            if (type.TypeKind == TypeKind.Class &&
                type.Name == "List" &&
                type.ContainingNamespace.ToString() == "System.Collections.Generic" &&
                type is INamedTypeSymbol listType &&
                listType.IsGenericType &&
                listType.TypeArguments.Length == 1)
            {
                typeArgument = listType.TypeArguments[0];
                return true;
            }

            typeArgument = null;
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
            if (type.TypeKind == TypeKind.Class &&
                type.ContainingAssembly.Name == "MassTransit")
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
    }
}