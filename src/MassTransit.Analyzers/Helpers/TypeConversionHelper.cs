namespace MassTransit.Analyzers.Helpers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;


    public class TypeConversionHelper
    {
        readonly SemanticModel _semanticModel;
        readonly NodeList<ITypeSymbol> _typeSymbols;
        INamedTypeSymbol _taskTypeSymbol;

        public TypeConversionHelper(SemanticModel semanticModel)
        {
            _semanticModel = semanticModel;
            _typeSymbols = new NodeList<ITypeSymbol>(100);

            _taskTypeSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(Task<>).FullName);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, SpecialType.System_Boolean);
            _typeSymbols.Add(semanticModel, SpecialType.System_Boolean, SpecialType.System_String, SpecialType.System_Object, SpecialType.System_SByte,
                SpecialType.System_Byte, SpecialType.System_Int16, SpecialType.System_UInt16, SpecialType.System_Int32, SpecialType.System_UInt32,
                SpecialType.System_Int64, SpecialType.System_UInt64);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, SpecialType.System_Byte);
            _typeSymbols.Add(semanticModel, SpecialType.System_Byte, SpecialType.System_String, SpecialType.System_Object, SpecialType.System_SByte,
                SpecialType.System_Int16, SpecialType.System_UInt16, SpecialType.System_Int32, SpecialType.System_UInt32,
                SpecialType.System_Int64, SpecialType.System_UInt64);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, SpecialType.System_Decimal);
            _typeSymbols.Add(semanticModel, SpecialType.System_Decimal, SpecialType.System_String, SpecialType.System_Object, SpecialType.System_SByte,
                SpecialType.System_Byte, SpecialType.System_Int16, SpecialType.System_UInt16, SpecialType.System_Int32, SpecialType.System_UInt32,
                SpecialType.System_Int64, SpecialType.System_UInt64);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, SpecialType.System_Double);
            _typeSymbols.Add(semanticModel, SpecialType.System_Double, SpecialType.System_String, SpecialType.System_Object, SpecialType.System_SByte,
                SpecialType.System_Byte, SpecialType.System_Int16, SpecialType.System_UInt16, SpecialType.System_Int32, SpecialType.System_UInt32,
                SpecialType.System_Int64, SpecialType.System_UInt64);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, SpecialType.System_Int32);
            _typeSymbols.Add(semanticModel, SpecialType.System_Int32, SpecialType.System_String, SpecialType.System_Object, SpecialType.System_SByte,
                SpecialType.System_Byte, SpecialType.System_Int16, SpecialType.System_UInt16, SpecialType.System_UInt32,
                SpecialType.System_Int64, SpecialType.System_UInt64);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, SpecialType.System_Int64);
            _typeSymbols.Add(semanticModel, SpecialType.System_Int64, SpecialType.System_String, SpecialType.System_Object, SpecialType.System_SByte,
                SpecialType.System_Byte, SpecialType.System_Int16, SpecialType.System_UInt16, SpecialType.System_Int32, SpecialType.System_UInt32,
                SpecialType.System_UInt64);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, SpecialType.System_Int16);
            _typeSymbols.Add(semanticModel, SpecialType.System_Int16, SpecialType.System_String, SpecialType.System_Object, SpecialType.System_SByte,
                SpecialType.System_Byte, SpecialType.System_UInt16, SpecialType.System_Int32, SpecialType.System_UInt32,
                SpecialType.System_Int64, SpecialType.System_UInt64);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, SpecialType.System_DateTime);
            _typeSymbols.Add(semanticModel, SpecialType.System_Int32, SpecialType.System_DateTime);
            _typeSymbols.Add(semanticModel, SpecialType.System_Int64, SpecialType.System_DateTime);
            _typeSymbols.Add(semanticModel, SpecialType.System_DateTime, SpecialType.System_String, SpecialType.System_Object, SpecialType.System_Int32,
                SpecialType.System_Int64);

            var dateTimeOffsetSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(DateTimeOffset).FullName);

            _typeSymbols.Add(semanticModel, SpecialType.System_DateTime, dateTimeOffsetSymbol);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, dateTimeOffsetSymbol);
            _typeSymbols.Add(semanticModel, SpecialType.System_Int32, dateTimeOffsetSymbol);
            _typeSymbols.Add(semanticModel, SpecialType.System_Int64, dateTimeOffsetSymbol);
            _typeSymbols.Add(semanticModel, dateTimeOffsetSymbol, SpecialType.System_String, SpecialType.System_Object, SpecialType.System_Int32,
                SpecialType.System_Int64);

            var timeSpanSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(TimeSpan).FullName);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, timeSpanSymbol);
            _typeSymbols.Add(semanticModel, SpecialType.System_Int32, timeSpanSymbol);
            _typeSymbols.Add(semanticModel, SpecialType.System_Int64, timeSpanSymbol);
            _typeSymbols.Add(semanticModel, timeSpanSymbol, SpecialType.System_String, SpecialType.System_Object, SpecialType.System_Int32,
                SpecialType.System_Int64);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, SpecialType.System_Enum);
            _typeSymbols.Add(semanticModel, SpecialType.System_Enum, SpecialType.System_String, SpecialType.System_Object, SpecialType.System_SByte,
                SpecialType.System_Byte, SpecialType.System_Int16, SpecialType.System_UInt16, SpecialType.System_Int32, SpecialType.System_UInt32,
                SpecialType.System_Int64, SpecialType.System_UInt64);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, SpecialType.System_Object);

            var exceptionSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(Exception).FullName);

            var exceptionInfoSymbol = semanticModel.Compilation.GetTypeByMetadataName("MassTransit.ExceptionInfo");
            if (exceptionInfoSymbol != null)
            {
                _typeSymbols.Add(exceptionInfoSymbol, exceptionSymbol);
                _typeSymbols.Add(semanticModel, SpecialType.System_String, exceptionInfoSymbol);
                _typeSymbols.Add(semanticModel, exceptionInfoSymbol, SpecialType.System_Object);
            }

            var uriSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(Uri).FullName);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, uriSymbol);
            _typeSymbols.Add(semanticModel, uriSymbol, SpecialType.System_Object, SpecialType.System_String);

            var versionSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(Version).FullName);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, versionSymbol);
            _typeSymbols.Add(semanticModel, versionSymbol, SpecialType.System_Object, SpecialType.System_String);

            var guidSymbol = semanticModel.Compilation.GetTypeByMetadataName(typeof(Guid).FullName);

            _typeSymbols.Add(semanticModel, SpecialType.System_String, guidSymbol);
            _typeSymbols.Add(semanticModel, guidSymbol, SpecialType.System_Object, SpecialType.System_String);

            var newIdSymbol = semanticModel.Compilation.GetTypeByMetadataName("MassTransit.NewId");
            if (newIdSymbol != null)
                _typeSymbols.Add(guidSymbol, newIdSymbol);
        }

        public bool CanConvert(Type type, ITypeSymbol sourceSymbol)
        {
            var symbol = _semanticModel.Compilation.GetTypeByMetadataName(type.FullName);

            return CanConvert(symbol, sourceSymbol);
        }

        public bool CanConvert(ITypeSymbol symbol, ITypeSymbol sourceSymbol)
        {
            while (true)
            {
                if (SymbolEqualityComparer.Default.Equals(symbol, sourceSymbol))
                    return true;

                if (symbol.IsNullable(out var underlyingSymbol))
                {
                    symbol = underlyingSymbol;
                    continue;
                }

                if (sourceSymbol.IsNullable(out var underlyingSourceSymbol))
                {
                    sourceSymbol = underlyingSourceSymbol;
                    continue;
                }

                if (sourceSymbol.IsInVar(out var variableType))
                {
                    sourceSymbol = variableType;
                    continue;
                }

                if (IsTask(sourceSymbol, out var taskType))
                {
                    sourceSymbol = taskType;
                    continue;
                }

                if (IsMessageData(symbol, out var messageDataType))
                {
                    if (messageDataType.IsArray(out var arrayType) && arrayType.SpecialType == SpecialType.System_Byte)
                    {
                        if (IsMessageData(sourceSymbol, out var sourceMessageDataType))
                        {
                            if (sourceMessageDataType.IsArray(out var sourceDataArrayType) && sourceDataArrayType.SpecialType == SpecialType.System_Byte)
                                return true;

                            if (sourceMessageDataType.SpecialType == SpecialType.System_String)
                                return true;
                        }

                        if (sourceSymbol.IsArray(out var sourceArrayType) && sourceArrayType.SpecialType == SpecialType.System_Byte)
                            return true;

                        if (sourceSymbol.SpecialType == SpecialType.System_String)
                            return true;
                    }

                    if (messageDataType.SpecialType == SpecialType.System_String && sourceSymbol.SpecialType == SpecialType.System_String)
                        return true;

                    INamedTypeSymbol streamType = _semanticModel.Compilation.GetTypeByMetadataName(typeof(Stream).FullName);
                    if (SymbolEqualityComparer.Default.Equals(messageDataType, streamType) && sourceSymbol.ImplementsType(streamType))
                        return true;

                    return false;
                }

                if (sourceSymbol.InheritsFromType(symbol))
                    return true;

                return _typeSymbols.Contains(symbol, sourceSymbol);
            }
        }

        static bool IsTask(ITypeSymbol symbol, out ITypeSymbol result)
        {
            if (symbol.TypeKind == TypeKind.Class
                && symbol.Name == "Task"
                && symbol.ContainingNamespace.Name == "Tasks"
                && symbol.ContainingNamespace.ContainingNamespace.Name == "Threading"
                && symbol.ContainingNamespace.ContainingNamespace.ContainingNamespace.Name == "System"
                && symbol is INamedTypeSymbol taskTypeSymbol
                && taskTypeSymbol.IsGenericType
                && taskTypeSymbol.TypeArguments.Length == 1)
            {
                result = taskTypeSymbol.TypeArguments[0];
                return true;
            }

            result = default;
            return false;
        }

        static bool IsMessageData(ITypeSymbol symbol, out ITypeSymbol result)
        {
            if (symbol.TypeKind == TypeKind.Interface
                && symbol.Name == "MessageData"
                && symbol.ContainingNamespace.Name == "MassTransit"
                && symbol is INamedTypeSymbol messageDataTypeSymbol
                && messageDataTypeSymbol.IsGenericType
                && messageDataTypeSymbol.TypeArguments.Length == 1)
            {
                result = messageDataTypeSymbol.TypeArguments[0];
                return true;
            }

            result = default;
            return false;
        }
    }


    static class NodeListExtensions
    {
        public static void Add(this NodeList<ITypeSymbol> table, SemanticModel semanticModel, ITypeSymbol symbol, params SpecialType[] types)
        {
            ITypeSymbol[] typeSymbols = types.Select(type => semanticModel.Compilation.GetSpecialType(type)).Cast<ITypeSymbol>().ToArray();

            table.Add(symbol, typeSymbols);
        }

        public static void Add(this NodeList<ITypeSymbol> table, SemanticModel semanticModel, SpecialType specialType, params SpecialType[] types)
        {
            var specialTypeSymbol = semanticModel.Compilation.GetSpecialType(specialType);
            ITypeSymbol[] typeSymbols = types.Select(type => semanticModel.Compilation.GetSpecialType(type)).Cast<ITypeSymbol>().ToArray();

            table.Add(specialTypeSymbol, typeSymbols);
        }

        public static void Add(this NodeList<ITypeSymbol> table, SemanticModel semanticModel, SpecialType specialType, params ITypeSymbol[] typeSymbols)
        {
            var specialTypeSymbol = semanticModel.Compilation.GetSpecialType(specialType);

            table.Add(specialTypeSymbol, typeSymbols);
        }
    }
}
