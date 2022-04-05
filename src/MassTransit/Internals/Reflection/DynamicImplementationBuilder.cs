namespace MassTransit.Internals
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;


    public class DynamicImplementationBuilder :
        IImplementationBuilder
    {
        const MethodAttributes PropertyAccessMethodAttributes = MethodAttributes.Public
            | MethodAttributes.SpecialName
            | MethodAttributes.HideBySig
            | MethodAttributes.Final
            | MethodAttributes.Virtual
            | MethodAttributes.VtableLayoutMask;

        readonly ConcurrentDictionary<string, ModuleBuilder> _moduleBuilders;
        readonly string _proxyNamespaceSuffix = "MassTransit.DynamicInternal" + Guid.NewGuid().ToString("N");
        readonly ConcurrentDictionary<Type, Lazy<Type>> _proxyTypes;

        public DynamicImplementationBuilder()
        {
            _moduleBuilders = new ConcurrentDictionary<string, ModuleBuilder>();

            _proxyTypes = new ConcurrentDictionary<Type, Lazy<Type>>();
        }

        public Type GetImplementationType(Type interfaceType)
        {
            return _proxyTypes.GetOrAdd(interfaceType, x => new Lazy<Type>(() => CreateImplementation(x))).Value;
        }

        Type CreateImplementation(Type interfaceType)
        {
            if (!interfaceType.GetTypeInfo().IsInterface)
                throw new ArgumentException("Proxies can only be created for interfaces: " + interfaceType.Name, nameof(interfaceType));

            return GetModuleBuilderForType(interfaceType, moduleBuilder => CreateTypeFromInterface(moduleBuilder, interfaceType));
        }

        static Type CreateTypeFromInterface(ModuleBuilder builder, Type interfaceType)
        {
            var typeName = "MassTransit.DynamicInternal." +
                (interfaceType.IsNested && interfaceType.DeclaringType != null
                    ? $"{interfaceType.DeclaringType.Name}+{TypeCache.GetShortName(interfaceType)}"
                    : TypeCache.GetShortName(interfaceType));
            try
            {
                var typeBuilder = builder.DefineType(typeName,
                    TypeAttributes.Serializable | TypeAttributes.Class |
                    TypeAttributes.Public | TypeAttributes.Sealed,
                    typeof(object), new[] { interfaceType });

                typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

                IEnumerable<PropertyInfo> properties = interfaceType.GetAllProperties();
                foreach (var property in properties)
                {
                    var fieldBuilder = typeBuilder.DefineField("field_" + property.Name, property.PropertyType,
                        FieldAttributes.Private);

                    var propertyBuilder = typeBuilder.DefineProperty(property.Name,
                        property.Attributes | PropertyAttributes.HasDefault, property.PropertyType, null);

                    foreach (var attributeData in property.GetCustomAttributesData())
                        propertyBuilder.SetCustomAttribute(GetCustomAttributeBuilder(attributeData));

                    var getMethod = GetGetMethodBuilder(property, typeBuilder, fieldBuilder);
                    var setMethod = GetSetMethodBuilder(property, typeBuilder, fieldBuilder);

                    propertyBuilder.SetGetMethod(getMethod);
                    propertyBuilder.SetSetMethod(setMethod);
                }

                return typeBuilder.CreateTypeInfo().AsType();
            }
            catch (Exception ex)
            {
                var message = $"Exception creating proxy ({typeName}) for {TypeCache.GetShortName(interfaceType)}";

                throw new InvalidOperationException(message, ex);
            }
        }

        static MethodBuilder GetGetMethodBuilder(PropertyInfo propertyInfo, TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
        {
            var getMethodBuilder = typeBuilder.DefineMethod("get_" + propertyInfo.Name,
                PropertyAccessMethodAttributes,
                propertyInfo.PropertyType,
                Type.EmptyTypes);

            var il = getMethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldBuilder);
            il.Emit(OpCodes.Ret);

            return getMethodBuilder;
        }

        static MethodBuilder GetSetMethodBuilder(PropertyInfo propertyInfo, TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
        {
            var setMethodBuilder = typeBuilder.DefineMethod("set_" + propertyInfo.Name,
                PropertyAccessMethodAttributes,
                null,
                new[] { propertyInfo.PropertyType });

            var il = setMethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldBuilder);
            il.Emit(OpCodes.Ret);

            return setMethodBuilder;
        }

        static CustomAttributeBuilder GetCustomAttributeBuilder(CustomAttributeData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var propertyArguments = new List<PropertyInfo>();
            var propertyArgumentValues = new List<object>();
            var fieldArguments = new List<FieldInfo>();
            var fieldArgumentValues = new List<object>();
            foreach (var namedArg in data.NamedArguments)
            {
                var argName = namedArg.MemberName;
                var fi = data.AttributeType.GetField(argName);
                var pi = data.AttributeType.GetProperty(argName);

                if (fi != null)
                {
                    fieldArguments.Add(fi);
                    fieldArgumentValues.Add(namedArg.TypedValue.Value);
                }
                else if (pi != null)
                {
                    propertyArguments.Add(pi);
                    propertyArgumentValues.Add(namedArg.TypedValue.Value);
                }
            }

            return new CustomAttributeBuilder(
                data.Constructor,
                data.ConstructorArguments.Select(ctorArg => ctorArg.Value).ToArray(),
                propertyArguments.ToArray(),
                propertyArgumentValues.ToArray(),
                fieldArguments.ToArray(),
                fieldArgumentValues.ToArray());
        }

        TResult GetModuleBuilderForType<TResult>(Type interfaceType, Func<ModuleBuilder, TResult> callback)
        {
            var assemblyName = interfaceType.Namespace + _proxyNamespaceSuffix;

            var builder = _moduleBuilders.GetOrAdd(assemblyName, name =>
            {
                const AssemblyBuilderAccess access = AssemblyBuilderAccess.RunAndCollect;

                var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name), access);

                var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);

                return moduleBuilder;
            });

            return callback(builder);
        }
    }
}
