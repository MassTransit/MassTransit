// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Serialization.Custom
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;
	using Magnum.Extensions;
	using Magnum.Threading;

	public static class InterfaceImplementationBuilder
	{
		private static readonly ReaderWriterLockedDictionary<Type, Type> _proxyTypes = new ReaderWriterLockedDictionary<Type, Type>();

		private const string ProxyNamespaceSuffix = ".DynamicImpl";
		private const MethodAttributes PropertyAccessMethodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.VtableLayoutMask;

		public static Type GetProxyFor(Type typeToProxy)
		{
			return _proxyTypes.Retrieve(typeToProxy, () =>
				{
					ModuleBuilder moduleBuilder = GetModuleBuilderForType(typeToProxy);

					return BuildTypeProxy(moduleBuilder, typeToProxy);
				});
		}

		private static Type BuildTypeProxy(ModuleBuilder builder, Type typeToProxy)
		{
			if (!typeToProxy.IsInterface)
				throw new ArgumentException("Proxies can only be created for interfaces: " + typeToProxy.Name, "typeToProxy");

			Type proxyType = CreateTypeFromInterface(builder, typeToProxy);

			return proxyType;
		}

		private static Type CreateTypeFromInterface(ModuleBuilder builder, Type typeToProxy)
		{
			string typeName = typeToProxy.Namespace + ProxyNamespaceSuffix + "." + typeToProxy.Name;

			TypeBuilder typeBuilder = builder.DefineType(typeName, TypeAttributes.Serializable | TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed, typeof (object), new[]{typeToProxy});

			typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

			typeToProxy.GetAllProperties().Each(x =>
				{
					FieldBuilder fieldBuilder = typeBuilder.DefineField("field_" + x.Name, x.PropertyType, FieldAttributes.Private);

					PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(x.Name, x.Attributes | PropertyAttributes.HasDefault, x.PropertyType, null);

					var getMethod = GetGetMethodBuilder(x, typeBuilder, fieldBuilder);
					var setMethod = GetSetMethodBuilder(x, typeBuilder, fieldBuilder);

					propertyBuilder.SetGetMethod(getMethod);
					propertyBuilder.SetSetMethod(setMethod);
				});

			//typeBuilder.AddInterfaceImplementation(typeToProxy);

			return typeBuilder.CreateType();
		}

		private static MethodBuilder GetGetMethodBuilder(PropertyInfo propertyInfo, TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
		{
			MethodBuilder getMethodBuilder = typeBuilder.DefineMethod(
				"get_" + propertyInfo.Name,
				PropertyAccessMethodAttributes,
				propertyInfo.PropertyType,
				Type.EmptyTypes);

			ILGenerator il = getMethodBuilder.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, fieldBuilder);
			il.Emit(OpCodes.Ret);

			return getMethodBuilder;
		}

		private static MethodBuilder GetSetMethodBuilder(PropertyInfo propertyInfo, TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
		{
			MethodBuilder setMethodBuilder = typeBuilder.DefineMethod(
				"set_" + propertyInfo.Name,
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

		private static ModuleBuilder GetModuleBuilderForType(Type typeToProxy)
		{
			string assemblyName = typeToProxy.Namespace + ProxyNamespaceSuffix;

			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);

			return assemblyBuilder.DefineDynamicModule(assemblyName);
		}
	}
}