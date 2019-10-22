namespace MassTransit.Internals.Reflection
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Reflection.Emit;
    using Util;


    public class DynamicContractTypeBuilder :
        IContractTypeBuilder
    {
        const MethodAttributes PropertyAccessMethodAttributes = MethodAttributes.Public
            | MethodAttributes.SpecialName
            | MethodAttributes.HideBySig
            | MethodAttributes.Final
            | MethodAttributes.Virtual
            | MethodAttributes.VtableLayoutMask;

        readonly ConcurrentDictionary<string, ModuleBuilder> _moduleBuilders;
        readonly string _proxyNamespaceSuffix = ".MassTransit.DynamicContract" + FormatUtil.Formatter.Format(NewId.Next().ToByteArray());
        readonly ConcurrentDictionary<string, Lazy<Type>> _proxyTypes;

        public DynamicContractTypeBuilder()
        {
            _moduleBuilders = new ConcurrentDictionary<string, ModuleBuilder>();

            _proxyTypes = new ConcurrentDictionary<string, Lazy<Type>>();
        }

        public Type GetContractType(Contract contract)
        {
            return _proxyTypes.GetOrAdd(contract.Name, _ => new Lazy<Type>(() => CreateImplementation(contract))).Value;
        }

        Type CreateImplementation(Contract contract)
        {
            var (name, ns) = ParseMessageUrn(contract.Name);

            if (string.IsNullOrWhiteSpace(ns))
                throw new ArgumentException("The contract type did not have a valid namespace", nameof(contract));

            return GetModuleBuilderForType(ns, moduleBuilder => CreateTypeFromInterface(moduleBuilder, contract, name, ns));
        }

        (string name, string ns) ParseMessageUrn(string messageType)
        {
            if (Uri.TryCreate(messageType, UriKind.Absolute, out var type))
            {
                if (type.Segments.Length == 0)
                    return (null, null);

                string[] names = type.Segments[0].Split(':');
                if (names[0] != "message")
                    return (null, null);

                if (names.Length == 2)
                    return (names[1], null);

                if (names.Length >= 3)
                    return (names[2], names[1]);
            }

            return (null, null);
        }

        Type CreateTypeFromInterface(ModuleBuilder builder, Contract contract, string name, string ns)
        {
            var typeName = "MassTransit.DynamicContract." + (string.IsNullOrWhiteSpace(ns) ? name : $"{ns}.{name}");

            try
            {
                var typeBuilder = builder.DefineType(typeName,
                    TypeAttributes.Serializable | TypeAttributes.Class |
                    TypeAttributes.Public | TypeAttributes.Sealed,
                    typeof(object));

                typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

                Property[] properties = contract.Properties;
                foreach (var property in properties)
                {
                    var fieldBuilder = typeBuilder.DefineField("field_" + property.Name, property.PropertyType, FieldAttributes.Private);

                    var propertyBuilder = typeBuilder.DefineProperty(property.Name, PropertyAttributes.HasDefault, property.PropertyType, null);

                    var getMethod = GetGetMethodBuilder(property, typeBuilder, fieldBuilder);
                    var setMethod = GetSetMethodBuilder(property, typeBuilder, fieldBuilder);

                    propertyBuilder.SetGetMethod(getMethod);
                    propertyBuilder.SetSetMethod(setMethod);
                }

                return typeBuilder.CreateTypeInfo().AsType();
            }
            catch (Exception ex)
            {
                string message = $"Exception creating proxy ({typeName}) for {contract.Name}";

                throw new InvalidOperationException(message, ex);
            }
        }

        MethodBuilder GetGetMethodBuilder(Property property, TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
        {
            var getMethodBuilder = typeBuilder.DefineMethod("get_" + property.Name,
                PropertyAccessMethodAttributes,
                property.PropertyType,
                Type.EmptyTypes);

            var il = getMethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldBuilder);
            il.Emit(OpCodes.Ret);

            return getMethodBuilder;
        }

        MethodBuilder GetSetMethodBuilder(Property property, TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
        {
            var setMethodBuilder = typeBuilder.DefineMethod("set_" + property.Name,
                PropertyAccessMethodAttributes,
                null,
                new[] {property.PropertyType});

            var il = setMethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldBuilder);
            il.Emit(OpCodes.Ret);

            return setMethodBuilder;
        }

        TResult GetModuleBuilderForType<TResult>(string ns, Func<ModuleBuilder, TResult> callback)
        {
            var assemblyName = ns + _proxyNamespaceSuffix;

            var builder = _moduleBuilders.GetOrAdd(assemblyName, name =>
            {
                const AssemblyBuilderAccess access = AssemblyBuilderAccess.RunAndCollect;

            #if NETCORE
                var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name), access);
            #else
                var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(name), access);
            #endif

                var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);

                return moduleBuilder;
            });

            return callback(builder);
        }
    }
}
