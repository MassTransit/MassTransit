// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Internals.Reflection
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using Extensions;
    using Util;


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
        readonly string _proxyNamespaceSuffix = ".DynamicInternal" + Guid.NewGuid().ToString("N");
        readonly ConcurrentDictionary<Type, Type> _proxyTypes;

        public DynamicImplementationBuilder()
        {
            _moduleBuilders = new ConcurrentDictionary<string, ModuleBuilder>();

            _proxyTypes = new ConcurrentDictionary<Type, Type>();
        }

        public Type GetImplementationType(Type interfaceType)
        {
            return _proxyTypes.GetOrAdd(interfaceType, CreateImplementation);
        }

        Type CreateImplementation(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException("Proxies can only be created for interfaces: " + interfaceType.Name,
                    "interfaceType");
            }

            return GetModuleBuilderForType(interfaceType, moduleBuilder => CreateTypeFromInterface(moduleBuilder, interfaceType));
        }

        Type CreateTypeFromInterface(ModuleBuilder builder, Type interfaceType)
        {
            string typeName = interfaceType.Namespace + _proxyNamespaceSuffix + "." +
                (interfaceType.IsNested && interfaceType.DeclaringType != null
                    ? (interfaceType.DeclaringType.Name + '+' + interfaceType.Name)
                    : interfaceType.Name);
            try
            {
                TypeBuilder typeBuilder = builder.DefineType(typeName,
                    TypeAttributes.Serializable | TypeAttributes.Class |
                        TypeAttributes.Public | TypeAttributes.Sealed,
                    typeof(object), new[] {interfaceType});

                typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

                IEnumerable<PropertyInfo> properties = interfaceType.GetAllProperties();
                foreach (PropertyInfo property in properties)
                {
                    FieldBuilder fieldBuilder = typeBuilder.DefineField("field_" + property.Name, property.PropertyType,
                        FieldAttributes.Private);

                    PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(property.Name,
                        property.Attributes | PropertyAttributes.HasDefault, property.PropertyType, null);

                    MethodBuilder getMethod = GetGetMethodBuilder(property, typeBuilder, fieldBuilder);
                    MethodBuilder setMethod = GetSetMethodBuilder(property, typeBuilder, fieldBuilder);

                    propertyBuilder.SetGetMethod(getMethod);
                    propertyBuilder.SetSetMethod(setMethod);
                }

                return typeBuilder.CreateType();
            }
            catch (Exception ex)
            {
                string message = string.Format("Exception creating proxy ({0}) for {1}", typeName, TypeMetadataCache.ShortName(interfaceType));

                throw new InvalidOperationException(message, ex);
            }
        }

        MethodBuilder GetGetMethodBuilder(PropertyInfo propertyInfo, TypeBuilder typeBuilder,
            FieldBuilder fieldBuilder)
        {
            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("get_" + propertyInfo.Name,
                PropertyAccessMethodAttributes,
                propertyInfo.PropertyType,
                Type.EmptyTypes);

            ILGenerator il = getMethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldBuilder);
            il.Emit(OpCodes.Ret);

            return getMethodBuilder;
        }

        MethodBuilder GetSetMethodBuilder(PropertyInfo propertyInfo, TypeBuilder typeBuilder,
            FieldBuilder fieldBuilder)
        {
            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("set_" + propertyInfo.Name,
                PropertyAccessMethodAttributes,
                null,
                new[] {propertyInfo.PropertyType});

            ILGenerator il = setMethodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldBuilder);
            il.Emit(OpCodes.Ret);

            return setMethodBuilder;
        }

        TResult GetModuleBuilderForType<TResult>(Type interfaceType, Func<ModuleBuilder, TResult> callback)
        {
            string assemblyName = interfaceType.Namespace + _proxyNamespaceSuffix;

            ModuleBuilder builder = _moduleBuilders.GetOrAdd(assemblyName, name =>
            {
                const AssemblyBuilderAccess access = AssemblyBuilderAccess.RunAndCollect;
                AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(name), access);

                ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);

                return moduleBuilder;
            });

            return callback(builder);
        }
    }
}