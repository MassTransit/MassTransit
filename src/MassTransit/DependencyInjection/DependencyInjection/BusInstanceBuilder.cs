namespace MassTransit.DependencyInjection
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using Internals;
    using NewIdFormatters;


    public class BusInstanceBuilder :
        IBusInstanceBuilder
    {
        const MethodAttributes PropertyAccessMethodAttributes = MethodAttributes.Public
            | MethodAttributes.SpecialName
            | MethodAttributes.HideBySig
            | MethodAttributes.Final
            | MethodAttributes.Virtual
            | MethodAttributes.VtableLayoutMask;

        public static readonly IBusInstanceBuilder Instance = Cached.Builder;

        readonly ConcurrentDictionary<string, ModuleBuilder> _moduleBuilders;
        readonly string _proxyNamespaceSuffix = ".MassTransit.BusInstances" + NewId.Next().ToString(ZBase32Formatter.LowerCase);
        readonly ConcurrentDictionary<Type, Lazy<Type>> _proxyTypes;

        BusInstanceBuilder()
        {
            _moduleBuilders = new ConcurrentDictionary<string, ModuleBuilder>();

            _proxyTypes = new ConcurrentDictionary<Type, Lazy<Type>>();
        }

        public TResult GetBusInstanceType<TBus, TResult>(IBusInstanceBuilderCallback<TBus, TResult> callback)
            where TBus : class, IBus
        {
            var interfaceType = typeof(TBus);

            var busInstanceType = _proxyTypes.GetOrAdd(interfaceType, x => new Lazy<Type>(() => CreateImplementation(x))).Value;

            var result = (TResult)typeof(IBusInstanceBuilderCallback<TBus, TResult>)
                .GetMethod("GetResult")
                .MakeGenericMethod(busInstanceType)
                .Invoke(callback, Array.Empty<object>());

            return result;
        }

        Type CreateImplementation(Type interfaceType)
        {
            var typeInfo = interfaceType.GetTypeInfo();

            if (!typeInfo.IsInterface)
                throw new ArgumentException("Bus instance types can only be created for interfaces: " + interfaceType.Name, nameof(interfaceType));

            if (typeInfo.IsGenericType)
                throw new ArgumentException("Bus instance types can not be generic: " + interfaceType.Name, nameof(interfaceType));

            if (!typeInfo.HasInterface<IBus>())
                throw new ArgumentException("Bus instance types must include the IBus interface: " + interfaceType.Name, nameof(interfaceType));

            return GetModuleBuilderForType(interfaceType, moduleBuilder => CreateTypeFromInterface(moduleBuilder, interfaceType));
        }

        Type CreateTypeFromInterface(ModuleBuilder builder, Type interfaceType)
        {
            var classTypeName = interfaceType.Name.StartsWith("I") ? interfaceType.Name.Substring(1) : interfaceType.Name + "Instance";

            var ns = interfaceType.IsNested && interfaceType.DeclaringType != null
                ? interfaceType.DeclaringType.Namespace
                : interfaceType.Namespace;
            if (ns != null)
                ns += ".";

            var typeName = "MassTransit.BusInstances." +
                (interfaceType.IsNested && interfaceType.DeclaringType != null
                    ? $"{ns}{interfaceType.DeclaringType.Name}+{classTypeName}"
                    : $"{ns}{classTypeName}");
            try
            {
                var parentType = typeof(BusInstance<>).MakeGenericType(interfaceType);

                var typeBuilder = builder.DefineType(typeName,
                    TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed,
                    parentType, new[] { interfaceType });

                Type[] parameterTypes = { typeof(IBusControl) };

                var ctorParent = parentType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, parameterTypes, null);
                var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameterTypes);

                var il = ctorBuilder.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, ctorParent);
                il.Emit(OpCodes.Ret);

                Type[] extraInterfaces = interfaceType.GetTypeInfo().GetAllInterfaces().Except(typeof(IBus).GetTypeInfo().GetAllInterfaces()).ToArray();

                IEnumerable<PropertyInfo> properties = interfaceType.GetAllProperties();
                foreach (var property in properties)
                {
                    if (extraInterfaces.Contains(property.DeclaringType))
                    {
                        var fieldBuilder = typeBuilder.DefineField("field_" + property.Name, property.PropertyType,
                            FieldAttributes.Private);

                        var propertyBuilder = typeBuilder.DefineProperty(property.Name,
                            property.Attributes | PropertyAttributes.HasDefault, property.PropertyType, null);

                        var getMethod = GetGetMethodBuilder(property, typeBuilder, fieldBuilder);
                        var setMethod = GetSetMethodBuilder(property, typeBuilder, fieldBuilder);

                        propertyBuilder.SetGetMethod(getMethod);
                        propertyBuilder.SetSetMethod(setMethod);
                    }
                }

                return typeBuilder.CreateTypeInfo().AsType();
            }
            catch (Exception ex)
            {
                var message = $"Exception creating bus instance ({typeName}) for {TypeCache.GetShortName(interfaceType)}";

                throw new InvalidOperationException(message, ex);
            }
        }

        MethodBuilder GetGetMethodBuilder(PropertyInfo propertyInfo, TypeBuilder typeBuilder,
            FieldBuilder fieldBuilder)
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

        MethodBuilder GetSetMethodBuilder(PropertyInfo propertyInfo, TypeBuilder typeBuilder,
            FieldBuilder fieldBuilder)
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


        static class Cached
        {
            internal static readonly IBusInstanceBuilder Builder = new BusInstanceBuilder();
        }
    }
}
